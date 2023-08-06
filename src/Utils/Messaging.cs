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
        ServerChatUtils.SendSystemMessageToClient(em, user, message);
    }

    public static void SendGlobalMessage(ServerChatMessageType msgType, string message)
    {
        var userQuery = em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>())).ToEntityArray(Allocator.Temp);

        foreach (var entity in userQuery)
        {
            em.TryGetComponentData<User>(entity, out var user);
            if (!user.IsConnected) continue;
            SendMessage(user, msgType, message);
        }
    }

}