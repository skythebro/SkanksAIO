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
        /*
        var entity = em.CreateEntity(
            ComponentType.ReadOnly<NetworkEventType>(),      //event type
            ComponentType.ReadOnly<SendEventToUser>(),       //send it to user
            ComponentType.ReadOnly<ChatMessageServerEvent>() // what event
        );

        NetworkId nid = em.GetComponentData<NetworkId>(user.LocalCharacter._Entity);

        var ev1 = new ChatMessageServerEvent
        {
            MessageText = message,
            MessageType = msgType,
            FromUser = nid,
            TimeUTC = DateTime.Now.ToFileTimeUtc()
        };

        entity.WithComponentData((ref SendEventToUser eventToUser) =>
        {
            eventToUser.UserIndex = user.Index;
        });
        
        entity.WithComponentData((ref NetworkEventType net) =>
        {
            net.EventId = NetworkEvents.EventId_ChatMessageServerEvent;
            net.IsAdminEvent = false;
            net.IsDebugEvent = false;
        });

        //fire off the event
        em.SetComponentData(entity, ev1);
        */
    }

    public static void SendMessage(Entity userEntity, ServerChatMessageType msgType, string message)
    {
        var entity = em.CreateEntity(
            ComponentType.ReadOnly<NetworkEventType>(),      //event type
            ComponentType.ReadOnly<SendEventToUser>(),       //send it to user
            ComponentType.ReadOnly<ChatMessageServerEvent>() // what event
        );

        em.TryGetComponentData<User>(userEntity, out var user);
        em.TryGetComponentData<NetworkId>(userEntity, out var nid);

        var ev1 = new ChatMessageServerEvent
        {
            MessageText = message,
            MessageType = msgType,
            FromUser = nid,
            TimeUTC = DateTime.Now.ToFileTimeUtc()
        };

        entity.WithComponentData((ref SendEventToUser eventToUser) =>
        {
            eventToUser.UserIndex = user.Index;
        });

        entity.WithComponentData((ref NetworkEventType net) =>
        {
            net.EventId = NetworkEvents.EventId_ChatMessageServerEvent;
            net.IsAdminEvent = false;
            net.IsDebugEvent = false;
        });
        
        //fire off the event
        em.SetComponentData(entity, ev1);
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