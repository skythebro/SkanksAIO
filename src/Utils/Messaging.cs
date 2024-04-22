using System;
using Bloodstone.API;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;

namespace SkanksAIO.Utils;

public static class Messaging
{
    private static EntityManager em = VWorld.Server.EntityManager;
    
    public static void SendMessage(User user, ServerChatMessageType msgType, string message)
    {
        if (msgType == ServerChatMessageType.Global)
        {
            ServerChatUtils.SendSystemMessageToAllClients(em, message);
        }
        else if (msgType == ServerChatMessageType.System)
        {
            ServerChatUtils.SendSystemMessageToClient(em, user, message);
        }
    }

    public static void SendMessage(Entity userEntity, ServerChatMessageType msgType, string message)
    {
        em.TryGetComponentData<User>(userEntity, out var user);
        SendMessage(user,msgType, message);
    }

    public static void SendGlobalMessage(ServerChatMessageType msgType, string message)
    {
        Plugin.Logger?.LogDebug("SendGlobalMessage method called");

        Plugin.Logger?.LogDebug("Creating user query");
        var userQuery = em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>())).ToEntityArray(Allocator.Temp);
        Plugin.Logger?.LogDebug("User query created");

        if (msgType == ServerChatMessageType.Global)
        {
            Plugin.Logger?.LogDebug("Sending global message");
            ServerChatUtils.SendSystemMessageToAllClients(em, message);
            Plugin.Logger?.LogDebug("Global message sent");
        }
        else if (msgType == ServerChatMessageType.System)
        {
            Plugin.Logger?.LogDebug("Sending system message to each user");
            foreach (var entity in userQuery)
            {
                Plugin.Logger?.LogDebug("Getting user component data");
                em.TryGetComponentData<User>(entity, out var user);
                Plugin.Logger?.LogDebug("User component data retrieved");

                if (!user.IsConnected) continue;

                SendMessage(user, msgType, message);
            }
            Plugin.Logger?.LogDebug("System message sent to each user");
        }
    }

}