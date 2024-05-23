using System;
using System.Threading.Tasks;
using Bloodstone.API;
using Bloodstone.Hooks;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using SkanksAIO;
using SkanksAIO.Utils;
using SkanksAIO.Utils.Config;
using Stunlock.Network.Steam.Wrapper;
using Unity.Collections;
using Unity.Entities;

[HarmonyPatch]
public static class Chat_Pathces
{
    [HarmonyPatch(typeof(ChatMessageSystem), nameof(ChatMessageSystem.OnUpdate))]
    [HarmonyPrefix]
    public static bool ChatUpdatePatch(ChatMessageSystem __instance)
    {
        var em = __instance.EntityManager;
        var query = __instance._ChatMessageQuery.ToEntityArray(Allocator.Temp);

        foreach (var entity in query)
        {
            if (!App.Instance!.Chat.Handle(entity))
            {
                // Hide the chat messages
                // Handle removing of message from query

                // return false; <-- Add this once the message is removed from the query.
                // Otherwise it will spam the command output in the chat.
                continue;
                // Avii's note: You *should not* return from a foreach loop, there might be more entries left, what happens with those?
                // It's a performance hit, if it needs to run through this entire thing again for the remaining entries.
                // Although I don't know how this stuff works under the hood, it'd be better to find a way without a return inside the loop.
            }

            em.TryGetComponentData<User>(entity, out var user);

            SendMessageToDiscord(em, entity, user.IsAdmin);
        }

        return true;
    }

    private static void SendMessageToDiscord(EntityManager em, Entity entity, bool isAdmin)
    {
        var chatMessageEvent = em.GetComponentDataAOT<ChatMessageEvent>(entity);
        if (chatMessageEvent.MessageType == ChatMessageType.Global)
        {
            
            string messageText = chatMessageEvent.MessageText.ToString() ?? string.Empty;

            
            // Check if the message starts with a dot
            if (messageText.StartsWith(".") || messageText.StartsWith(Settings.ChatCommandPrefix.ToString()))
            {
                // Do nothing user is trying to run a command
                return;
            }
            
            // Remove all @ symbols from the message
            var sanitizedMessage = messageText.Replace("@", "");

            
            Plugin.Logger?.LogDebug($"Just got a global message: {messageText}");
            var fromCharacter = em.GetComponentData<FromCharacter>(entity);
            var user = em.GetComponentData<User>(fromCharacter.User);

            if (isAdmin)
            {
                Plugin.Logger?.LogDebug($"[{Settings.GlobalChatLabel.Value}][Admin] {user.CharacterName}: {sanitizedMessage}");
                var _ = App.Instance.Discord.SendMessageAsync(
                    $"[{Settings.GlobalChatLabel.Value}][Admin]{user.CharacterName}: {sanitizedMessage}");
            }
            else
            {
                Plugin.Logger?.LogDebug($"[{Settings.GlobalChatLabel.Value}] {user.CharacterName}: {sanitizedMessage}");
                var _ = App.Instance.Discord.SendMessageAsync(
                    $"[{Settings.GlobalChatLabel.Value}] {user.CharacterName}: {sanitizedMessage}");
            }
        }
        else
        {
            Plugin.Logger?.LogDebug($"Just got a non global message: {chatMessageEvent.MessageText}");
        }
    }
}