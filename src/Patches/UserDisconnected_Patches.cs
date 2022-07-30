using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using SkanksAIO.Models;
using Stunlock.Network;

namespace SkanksAIO.Patches;

[HarmonyPatch]
public static class UserDisconnected_Patches
{
    [HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserDisconnected))]
    [HarmonyPrefix]
    public static void OnUserDisconnected_Patch(ServerBootstrapSystem __instance, NetConnectionId netConnectionId, ConnectionStatusChangeReason connectionStatusReason, string extraData)
    {
        var userIndex = __instance._NetEndPointToApprovedUserIndex[netConnectionId];
        var serverClient = __instance._ApprovedUsersLookup[userIndex];
        var userEntity = serverClient.UserEntity;

        var user = __instance.EntityManager.GetComponentData<User>(userEntity);

        var player = Player.GetRepository
            .FindOne(x => x.PlatformId == user.PlatformId);

        if (player == null || user.CharacterName.IsEmpty)
        {
            Plugin.Logger?.LogDebug("A user has disconnected from the Character Creation screen.");
            
            App.Instance?.Discord.SendMessageAsync("A player has disconnected from the Character Creation screen.");

            return;
        }

        Plugin.Logger?.LogDebug($"{player.CharacterName} disconnected.");
        App.Instance?.Discord.SendMessageAsync($"{player.CharacterName} disconnected.");
    }
}
