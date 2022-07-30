using System.Collections.Generic;
using ProjectM.Network;
using UnhollowerRuntimeLib;
using Unity.Collections;
using Unity.Entities;

namespace SkanksAIO.Utils;

class UserUtils
{
    public static bool TryGetUserByPlatformId(ulong platformId, out User? user)
    {
        var em = Plugin.World.EntityManager;
        var userQuery = em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>()));
        foreach (var entity in userQuery.ToEntityArray(Allocator.Temp))
        {
            var u = Plugin.World.EntityManager.GetComponentData<User>(entity);
            if (u.PlatformId == platformId)
            {
                user = u;
                return true;
            }
        }
        user = default;
        return false;
    }

    public static bool TryGetUserByCharacterName(string characterName, out User? user)
    {
        var em = Plugin.World.EntityManager;
        var userQuery = em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>()));
        foreach (var entity in userQuery.ToEntityArray(Allocator.Temp))
        {
            var u = Plugin.World.EntityManager.GetComponentData<User>(entity);
            if (u.CharacterName.ToString().ToLower() == characterName.ToLower())
            {
                user = u;
                return true;
            }
        }
        user = default;
        return false;
    }

    public static List<User> GetOnlineUsers()
    {
        // container to return
        var users = new List<User>();

        var em = Plugin.World.EntityManager;
        var userQuery = em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>()));
        foreach (var entity in userQuery.ToEntityArray(Allocator.Temp))
        {
            var u = Plugin.World.EntityManager.GetComponentData<User>(entity);

            if (u.IsConnected)
            {
                users.Add(u);
            }
        }

        return users;
    }
}
