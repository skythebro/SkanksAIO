using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using SkanksAIO.Models;
using Stunlock.Network;
using Unity.Entities;

namespace SkanksAIO.Patches;

[HarmonyPatch]
public static class InitializePlayer_Patches
{
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserConnected))]
    [HarmonyPostfix]
    public static void OnUserConnected_Patch(ServerBootstrapSystem __instance, NetConnectionId netConnectionId)
    {
        var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var serverClient = __instance._ApprovedUsersLookup[userIndex];
        var userEntity = serverClient.UserEntity;

        var user = __instance.EntityManager.GetComponentData<User>(userEntity);

        var player = Player.GetRepository
            .FindOne(x => x.PlatformId == user.PlatformId);

        if (player == null)
        {
            if (user.CharacterName.IsEmpty) {
                Plugin.Logger?.LogDebug("A new player has connected, wait for them to finish character creation.");
                App.Instance?.Discord.SendMessageAsync("A new player has connected");
                return;
            }
            player = new Player { PlatformId = user.PlatformId, CharacterName = user.CharacterName.ToString() };
            Player.GetRepository.Insert(player);
            
            Plugin.Logger?.LogDebug($"Creating new database entry for ({player.PlatformId}) {player.CharacterName}");
            App.Instance?.Discord.SendMessageAsync($"{player.CharacterName} has joined the fold.");

            return;
        }

        // An existing player has returned.
        Plugin.Logger?.LogDebug($"{player.CharacterName} connected");
        App.Instance?.Discord.SendMessageAsync($"{player.CharacterName} connected.");

        Plugin.Logger?.LogDebug("Name: " + player.CharacterName);
        Plugin.Logger?.LogDebug("PlatformId: " + player.PlatformId);
        Plugin.Logger?.LogDebug("Kills: " + player.Kills);
        Plugin.Logger?.LogDebug("Deaths: " + player.Deaths);
        Plugin.Logger?.LogDebug("KD: " + player.KD);
        Plugin.Logger?.LogDebug("ELO: " + player.ELO);
    }

    [HarmonyPatch(typeof(HandleCreateCharacterEventSystem), nameof(HandleCreateCharacterEventSystem.TryIsNameValid))]
    [HarmonyPostfix]
    public static void TryIsNameValid_Patch(bool __result, HandleCreateCharacterEventSystem __instance, Entity userEntity, string characterNameString)
    {
        if (!__result) return;

        Plugin.Logger?.LogDebug($"User ({characterNameString}) Picked a name successfully");

        var em = __instance._BootstrapSystem.EntityManager;

        var user = em.GetComponentData<User>(userEntity);

        var player = Player.GetRepository
            .FindOne(x => x.PlatformId == user.PlatformId);
        
        if (player == null) {
            player = new Player { PlatformId = user.PlatformId, CharacterName = characterNameString };
            Player.GetRepository.Insert(player);

            Plugin.Logger?.LogDebug($"Creating new database entry for ({player.PlatformId}) {player.CharacterName}");
            App.Instance?.Discord.SendMessageAsync($"{player.CharacterName} has joined the fold.");

            return;
        }

        player.CharacterName = characterNameString;
        player.Kills = 0;
        player.Deaths = 0;

        if (Player.GetRepository.Update(player)) {
            Plugin.Logger?.LogDebug($"Updating database entry for ({player.PlatformId}) {player.CharacterName}");
        } else {
            Plugin.Logger?.LogError($"Failed to update database entry for ({player.PlatformId}) {player.CharacterName}");
        }

        App.Instance?.Discord.SendMessageAsync($"{player.CharacterName} has joined the fold.");
    }
}
