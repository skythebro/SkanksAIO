using System;
using System.Linq;
using Bloodstone.API;
using HarmonyLib;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using SkanksAIO.Utils.Config;
using Stunlock.Network;
using Unity.Collections;
using Unity.Entities;

namespace SkanksAIO.Patches;

[HarmonyPatch]
public static class UserDisconnected_Patches
{
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserDisconnected))]
    [HarmonyPrefix]
    public static void OnUserDisconnected_Patch(ServerBootstrapSystem __instance, NetConnectionId netConnectionId,
        ConnectionStatusChangeReason connectionStatusReason, string extraData)
    {
        if (Settings.ShowUserDisConnectedInDc.Value)
        {
            if (__instance._NetEndPointToApprovedUserIndex.ContainsKey(netConnectionId))
            {
                var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
                var serverClient = __instance._ApprovedUsersLookup[userIndex];
                var userEntity = serverClient.UserEntity;

                var user = __instance.EntityManager.GetComponentData<User>(userEntity);

                var player = Player.GetPlayerRepository
                    .FindOne(x => x.PlatformId == user.PlatformId);
                if (player == null || user.CharacterName.IsEmpty)
                {
                    Plugin.Logger?.LogDebug("A user has disconnected from the Character Creation screen.");
                    var defMessage = JsonConfigHelper.GetDefaultMessage("newUserOffline");
                    App.Instance.Discord.SendMessageAsync(defMessage);
                    return;
                }

                Plugin.Logger?.LogDebug($"{player.CharacterName} disconnected.");
                var message = JsonConfigHelper.GetOfflineMessage(player.CharacterName!);
                message = message.Replace("%user%", player.CharacterName!);
                App.Instance.Discord.SendMessageAsync(message);
                player.IsConnected = false;
                if (Player.GetPlayerRepository.Update(player))
                {
                    Plugin.Logger?.LogDebug(
                        $"Updating database entry for ({player.PlatformId}) {player.CharacterName}");
                }
                else
                {
                    Plugin.Logger?.LogError(
                        $"Failed to update database entry for ({player.PlatformId}) {player.CharacterName}");
                }
            }
            else
            {
                // Plugin.Logger?.LogError(
                //     $"NetConnectionId {netConnectionId} not found in _NetEndPointToApprovedUserIndex dictionary.");
            }
        }
    }
}