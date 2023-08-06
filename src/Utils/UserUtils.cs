using System;
using System.Collections.Generic;
using System.Linq;
using Bloodstone.API;
using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using ProjectM.Terrain;
using SkanksAIO.Models;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;
using UnityEngine;

namespace SkanksAIO.Utils;

public class UserUtils
{
    private static EntityManager _em = VWorld.Server.EntityManager;

    public static bool TryGetUserByPlatformId(ulong platformId, out User? user)
    {
        var userQuery = _em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>()));
        foreach (var entity in userQuery.ToEntityArray(Allocator.Temp))
        {
            var u = VWorld.Server.EntityManager.GetComponentData<User>(entity);
            if (u.PlatformId == platformId)
            {
                user = u;
                return true;
            }
        }

        user = default;
        return false;
    }

    public static bool TryGetUserByCharacterName(string characterName, out User user)
    {
        var userQuery = _em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>()));
        foreach (var entity in userQuery.ToEntityArray(Allocator.Temp))
        {
            VWorld.Server.EntityManager.TryGetComponentData<User>(entity, out var userEnt);
            if (!string.Equals(userEnt.CharacterName.ToString().ToLower(), characterName.ToLower())) continue;
            user = userEnt;
            return true;
        }

        user = default;
        return false;
    }

    public static List<User> GetOnlineUsers()
    {
        // container to return
        var users = new List<User>();

        var userQuery = _em.CreateEntityQuery(ComponentType.ReadOnly(Il2CppType.Of<User>()));
        foreach (var entity in userQuery.ToEntityArray(Allocator.Temp))
        {
            if (!VWorld.Server.EntityManager.TryGetComponentData<User>(entity, out var user))
            {
                continue;
            }
            
            if (user.IsConnected)
            {
                users.Add(user);
            }
        }

        return users;
    }


    // Get all users as a List<User>
    public static List<User> GetUsers() => NativeArrayToList(_em
        .CreateEntityQuery(ComponentType.ReadWrite<User>())
        .ToComponentDataArray<User>(Allocator.Temp));

    // Convert User to Entity of Player
    public static Entity UserToPlayerEntity(User user) => user.LocalCharacter.GetEntityOnServer();

    // Get User by CharacterName (case-insensitive)
    public static User GetUserByCharacterName(String characterName) => GetUsers()
        .Find(user => String.Equals(user.CharacterName.ToString(), characterName,
            StringComparison.CurrentCultureIgnoreCase));

    // Get Users from contents of Message (case-insensitive)
    public static List<User> GetUsersByContainedCharacterName(String message) => GetUsers()
        .FindAll(user => message.ToLower().Contains(user.CharacterName.ToString().ToLower()));


// Convert NativeArray to List the expensive way
    public static List<T> NativeArrayToList<T>(NativeArray<T> array) where T : new()
    {
        List<T> list = new();
        foreach (T obj in array)
        {
            list.Add(obj);
        }

        return list;
    }

    // Assuming Player class has an identifier (e.g., PlatformId) and position (e.g., Vector3)
    private static Dictionary<ulong, Vector3> playerPositions = new();

// Method to update player position
    public static void UpdatePlayerPosition(ulong platformId, Vector3 position)
    {
        playerPositions[platformId] = position;
    }

    // Method to get player position
    public static Vector3 GetPlayerPosition(ulong platformId)
    {
        return playerPositions.TryGetValue(platformId, out var position) ? position : default; // Default position if not found
    }

    public static List<PlayerLocation> GetAllPlayerPositions()
    {
        var users = GetOnlineUsers();
        foreach (var user in users)
        {
            var tempPlayerCheck = Player.GetPlayerRepository
                .FindOne(x => x.PlatformId == user.PlatformId);
            if (tempPlayerCheck == null)
            {
                continue;
            }

            var databasePlayer = Player.GetPlayerRepository
                .FindOne(x => x.PlatformId == user.PlatformId);

            if (databasePlayer.PlatformId != user.PlatformId)
            {
                continue; // just in case
            }
            
            var player = UserToPlayerEntity(user);
            VWorld.Server.EntityManager.TryGetComponentData<Translation>(player, out var userpos);
            UpdatePlayerPosition(databasePlayer.PlatformId, userpos.Value);
        }

        var players = Player.GetPlayerRepository.FindAll();

        return (from player in players
            let pos = GetPlayerPosition(player.PlatformId)
            select new PlayerLocation()
            {
                Name = player.CharacterName, X = pos.x, Y = pos.y, Z = pos.z,
            }).ToList();
    }
    
    public class TerritoryData
    {
        public int PlayerCount { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        
        public float BX { get; set; }
        
        public float BY { get; set; }
        
        public float TX { get; set; }
        
        public float TY { get; set; }
    }
    
    public static Dictionary<string,TerritoryData> GetPlayerTerritory()
    {
        Dictionary<string,TerritoryData>territories = new();
        var userInRegionQuery = VWorld.Server.EntityManager.CreateEntityQuery(new EntityQueryDesc()
        {
            All = new[] { 
                ComponentType.ReadOnly<CurrentMapZone>(),
                ComponentType.ReadOnly<CurrentWorldRegion>(),
            },
            None = new[] { ComponentType.ReadOnly<DestroyTag>() }
        });

        var array = userInRegionQuery.ToEntityArray(Allocator.Temp);

        var territoriesInRepo = Territory.GetTerritoryRepository.FindAll();

        foreach (var ters in territoriesInRepo)
        {
            
            territories.Add(ters.territoryName!,new TerritoryData()
            {
                PlayerCount = 0,
                X = ters.CenterX,
                Y = ters.CenterZ,
                BX = ters.MinX,
                BY = ters.MinZ,
                TX = ters.MaxX,
                TY = ters.MaxZ,
            });
        }
        
        foreach (var user in array)
        {
            //var userZone = VWorld.Server.EntityManager.GetComponentData<CurrentMapZone>(user);
            var userRegion = VWorld.Server.EntityManager.GetComponentDataAOT<CurrentWorldRegion>(user);
            var userTerritory = Territory.GetTerritoryRepository.FindOne(x => x.territoryName == userRegion.CurrentRegion.ToString());
            if (userTerritory == null)
            {
                continue;
            }
            if (territories.ContainsKey(userTerritory.territoryName))
            {
                territories[userTerritory.territoryName].PlayerCount += 1;
                userTerritory.playersInRegion = territories[userTerritory.territoryName].PlayerCount;
                Territory.GetTerritoryRepository.Update(userTerritory);
            }
        }

        return territories;

    }
}