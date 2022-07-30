using System;
using ProjectM.Network;
using UnhollowerRuntimeLib;
using Unity.Collections;
using Unity.Entities;

namespace SkanksAIO.Utils;

public static class Messaging
{

    public static void SendMessage(User user, ServerChatMessageType msg_type, string message)
    {
        var em = Plugin.World.EntityManager;

        var entity = em.CreateEntity(
            ComponentType.ReadOnly<NetworkEventType>(),      //event type
            ComponentType.ReadOnly<SendEventToUser>(),       //send it to user
            ComponentType.ReadOnly<ChatMessageServerEvent>() // what event
        );

        NetworkId nid = em.GetComponentData<NetworkId>(user.LocalCharacter._Entity);

        var ev1 = new ChatMessageServerEvent();
        ev1.MessageText = message;
        ev1.MessageType = msg_type;
        ev1.FromUser = nid;
        ev1.TimeUTC = DateTime.Now.ToFileTimeUtc();

        em.SetComponentData<SendEventToUser>(entity, new()
        {
            UserIndex = user.Index
        });

        em.SetComponentData<NetworkEventType>(entity, new()
        {
            EventId = NetworkEvents.EventId_ChatMessageServerEvent,
            IsAdminEvent = false,
            IsDebugEvent = false
        });

        //fire off the event
        em.SetComponentData(entity, ev1);
    }

    public static void SendMessage(Entity userEntity, ServerChatMessageType msg_type, string message)
    {
        var em = Plugin.World.EntityManager;

        var entity = em.CreateEntity(
            ComponentType.ReadOnly<NetworkEventType>(),      //event type
            ComponentType.ReadOnly<SendEventToUser>(),       //send it to user
            ComponentType.ReadOnly<ChatMessageServerEvent>() // what event
        );

        var user = em.GetComponentData<User>(userEntity);
        var nid = em.GetComponentData<NetworkId>(userEntity);

        var ev1 = new ChatMessageServerEvent();
        ev1.MessageText = message;
        ev1.MessageType = msg_type;
        ev1.FromUser = nid;
        ev1.TimeUTC = DateTime.Now.ToFileTimeUtc();

        em.SetComponentData<SendEventToUser>(entity, new()
        {
            UserIndex = user.Index
        });

        em.SetComponentData<NetworkEventType>(entity, new()
        {
            EventId = NetworkEvents.EventId_ChatMessageServerEvent,
            IsAdminEvent = false,
            IsDebugEvent = false
        });

        //fire off the event
        em.SetComponentData(entity, ev1);
    }

    public static void SendGlobalMessage(ServerChatMessageType msg_type, string message)
    {
        var em = Plugin.World.EntityManager;

        var userQuery = em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>()));

        foreach (var entity in userQuery.ToEntityArray(Allocator.Temp))
        {
            var u = Plugin.World.EntityManager.GetComponentData<User>(entity);

            if (!u.IsConnected) continue;

            Messaging.SendMessage(u, msg_type, message);
        }
    }

}