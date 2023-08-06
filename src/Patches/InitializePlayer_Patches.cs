using System;
using System.Threading.Tasks;
using BepInEx.Core.Logging.Interpolation;
using BepInEx.Logging;
using Bloodstone.API;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using SkanksAIO.Models;
using SkanksAIO.Utils.Config;
using Stunlock.Network;
using Unity.Collections;
using Unity.Entities;

namespace SkanksAIO.Patches;

[HarmonyPatch]
public static class InitializePlayer_Patches
{
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserConnected))]
    [HarmonyPostfix]
    public static void OnUserConnected_Patch(ServerBootstrapSystem __instance, NetConnectionId netConnectionId)
    {
        try
        {
            Plugin.Logger?.LogDebug(VWorld.Server.GetExistingSystem<ServerBootstrapSystem>().ToString());

            var userIndex = VWorld.Server.GetExistingSystem<ServerBootstrapSystem>()._NetEndPointToApprovedUserIndex[netConnectionId];
            var serverClient = VWorld.Server.GetExistingSystem<ServerBootstrapSystem>()._ApprovedUsersLookup[userIndex];

            if (Settings.EnableVIPFunctionality.Value)
            {
                foreach (var num in JsonConfigHelper.GetVips())
                {
                    if (serverClient.PlatformId != num) continue;
                    VWorld.Server.EntityManager.TryGetComponentData<User>(serverClient.UserEntity, out var userData);
                    //var adminsystem = VWorld.Server.GetExistingSystem<AdminAuthSystem>();
                    //var isadmin = adminsystem.IsAdmin(serverClient.PlatformId);
                    userData.IsAdmin = false;
                }
            }


            var userEntity = serverClient.UserEntity;

            var user = VWorld.Server.EntityManager.GetComponentData<User>(userEntity);

            var player = Player.GetPlayerRepository
                .FindOne(x => x.PlatformId == user.PlatformId);
            if (player == null)
            {
                if (user.CharacterName.IsEmpty)
                {
                    Plugin.Logger?.LogDebug("A new player has connected, wait for them to finish character creation.");
                    if (Settings.ShowUserConnectedInDc.Value)
                    {
                        var message = JsonConfigHelper.GetOnlineMessage("");
                        App.Instance.Discord.SendMessageAsync(message);
                    }

                    return;
                }

                player = new Player { PlatformId = user.PlatformId, CharacterName = user.CharacterName.ToString() };
                Player.GetPlayerRepository.Insert(player);

                Plugin.Logger?.LogDebug(
                    $"Creating new database entry for ({player.PlatformId}) {player.CharacterName}");
                if (Settings.ShowUserConnectedInDc.Value)
                {
                    var message = JsonConfigHelper.GetOnlineMessage(player.CharacterName!);
                    message = message.Replace("%user%", player.CharacterName!);
                    App.Instance.Discord.SendMessageAsync(message);
                }

                return;
            }

            // An existing player has returned.
            Plugin.Logger?.LogDebug($"{player.CharacterName} connected");
            if (Settings.ShowUserConnectedInDc.Value)
            {
                var message = JsonConfigHelper.GetOnlineMessage(player.CharacterName!);
                message = message.Replace("%user%", player.CharacterName!);
                App.Instance.Discord.SendMessageAsync(message);
            }

            Plugin.Logger?.LogDebug("Name: " + player.CharacterName);
            Plugin.Logger?.LogDebug("PlatformId: " + player.PlatformId);
            Plugin.Logger?.LogDebug("Kills: " + player.Kills);
            Plugin.Logger?.LogDebug("Deaths: " + player.Deaths);
            Plugin.Logger?.LogDebug("KD: " + player.KD);
            Plugin.Logger?.LogDebug("ELO: " + player.ELO);
        }
        catch (NullReferenceException e)
        {
            Plugin.Logger?.LogError("Couldn't patch OnUserConnected. Error: " + e.Message);
        }
    }

    [HarmonyPatch(typeof(HandleCreateCharacterEventSystem), nameof(HandleCreateCharacterEventSystem.TryIsNameValid))]
    [HarmonyPostfix]
    public static void TryIsNameValid_Patch(bool __result, HandleCreateCharacterEventSystem __instance,
        Entity userEntity,
        string characterNameString)
    {
        if (!__result) return;

        Plugin.Logger?.LogDebug($"User ({characterNameString}) Picked a name successfully");

        var em =  VWorld.Server.GetExistingSystem<HandleCreateCharacterEventSystem>()._BootstrapSystem.EntityManager;

        var user = em.GetComponentData<User>(userEntity);

        var player = Player.GetPlayerRepository
            .FindOne(x => x.PlatformId == user.PlatformId);

        if (player == null)
        {
            player = new Player { PlatformId = user.PlatformId, CharacterName = characterNameString };
            Player.GetPlayerRepository.Insert(player);

            Plugin.Logger?.LogDebug($"Creating new database entry for ({player.PlatformId}) {player.CharacterName}");
            if (Settings.ShowUserConnectedInDc.Value)
            {
                
                var message = JsonConfigHelper.GetDefaultMessage("newUserCreatedCharacter");
                message = message.Replace("%user%", player.CharacterName!);
                App.Instance.Discord.SendMessageAsync(message);
            }

            return;
        }

        player.CharacterName = characterNameString;
        player.Kills = 0;
        player.Deaths = 0;

        if (Player.GetPlayerRepository.Update(player))
        {
            Plugin.Logger?.LogDebug($"Updating database entry for ({player.PlatformId}) {player.CharacterName}");
        }
        else
        {
            Plugin.Logger?.LogError(
                $"Failed to update database entry for ({player.PlatformId}) {player.CharacterName}");
        }

        if (Settings.ShowUserConnectedInDc.Value)
        {
            App.Instance?.Discord.SendMessageAsync($"{player.CharacterName} has joined the fold.");
        }
    }
}