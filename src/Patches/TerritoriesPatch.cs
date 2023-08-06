using System;
using System.Collections.Generic;
using Bloodstone.API;
using HarmonyLib;
using ProjectM;
using ProjectM.CastleBuilding;
using ProjectM.CastleBuilding.Placement;
using ProjectM.Network;
using ProjectM.Terrain;
using ProjectM.UI;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using Unity.Collections;
using Unity.Entities;

namespace SkanksAIO.Patches;

[HarmonyPatch]
public class TerritoriesPatch
{
    [HarmonyPatch(typeof(TerritoryOwnerCacheSystem), nameof(TerritoryOwnerCacheSystem.OnUpdate))]
    [HarmonyPrefix]
    public static void OnUpdate_patch(TerritoryOwnerCacheSystem __instance)
    {
        if (__instance._TerritoryOwnerCache.Count == 0)
        {
            return;
        }

        foreach (var keyValuePair in __instance._TerritoryOwnerCache)
        {
            Plugin.Logger?.LogInfo(
                $"TerritoryOwnerCache int: {keyValuePair.key} characterName: {keyValuePair.value.CharacterName}");
        }
    }

    static List<string> mapRegionEntities = new();
    static List<string> mapEntities = new();

    [HarmonyPatch(typeof(UpdateUserWorldRegionSystem), nameof(UpdateUserWorldRegionSystem.OnUpdate))]
    [HarmonyPrefix]
    public static void UpdateUserWorldRegionSystem_patch(UpdateUserWorldRegionSystem __instance)
    {
        // if (!__instance._MapRegionsQuery.IsEmpty)
        // {
        //     foreach (var mapRegionEntity in __instance._MapRegionsQuery.ToEntityArray(Allocator.Temp))
        //     {
        //         if (mapRegionEntities.Contains(mapRegionEntity.ToString()!)) continue;
        //         mapRegionEntities.Add(mapRegionEntity.ToString()!);
        //         Plugin.Logger?.LogWarning($"[mapRegionEntity][{mapRegionEntity.ToString()}]");
        //         var i = 1;
        //         foreach (var type in __instance.EntityManager.GetComponentTypes(mapRegionEntity))
        //         {
        //             Plugin.Logger?.LogWarning($"[{i++}][ComponentType][{type}]");
        //         }
        //     }
        // }
        
        // if (!__instance.__UpdateUsersCurrentMapRegions_entityQuery.IsEmpty)
        // {
        //     foreach (var userInMapRegion in __instance.__UpdateUsersCurrentMapRegions_entityQuery.ToEntityArray(
        //                  Allocator.Temp))
        //     {
        //         if (mapEntities.Contains(userInMapRegion.ToString()!)) continue;
        //         mapEntities.Add(userInMapRegion.ToString()!);
        //         Plugin.Logger?.LogWarning($"[UserInmapRegion][{userInMapRegion.ToString()}]");
        //         var i = 1;
        //         foreach (var type in __instance.EntityManager.GetComponentTypes(userInMapRegion))
        //         {
        //             Plugin.Logger?.LogWarning($"[{i++}][ComponentType][{type}]");
        //         }
        //         
        //         var hasuser =
        //             __instance.EntityManager.TryGetComponentData<User>(userInMapRegion, out var mapRegionData);
        //         if (!hasuser) continue;
        //         Plugin.Logger?.LogWarning($"[Name][{mapRegionData.CharacterName}]");
        //
        //         var currentWorldRegion =
        //             __instance.EntityManager.GetComponentDataAOT<CurrentWorldRegion>(userInMapRegion);
        //         Plugin.Logger?.LogWarning($"[LastValidRegion][{currentWorldRegion.LastValidRegion.ToString()}]");
        //         Plugin.Logger?.LogWarning($"[CurrentRegion][{currentWorldRegion.CurrentRegion.ToString()}]");
        //         
        //     }
        // }
    }

    [HarmonyPatch(typeof(SetTerritoryWorldRegionSystem), nameof(SetTerritoryWorldRegionSystem.OnUpdate))]
    [HarmonyPrefix]
    public static void SetTerritoryWorldRegionSystem_patch(SetTerritoryWorldRegionSystem __instance)
    {
        try
        {
            if (!__instance._GetWorldRegionQuery.IsEmpty)
            {
                foreach (var worldRegionEntity in __instance._GetWorldRegionQuery.ToEntityArray(Allocator.Temp))
                {
                    WorldRegionPolygon worldRegionPolygon =
                        __instance.EntityManager.GetComponentDataAOT<WorldRegionPolygon>(worldRegionEntity);
                    
                    //Plugin.Logger?.LogWarning($"worldRegionEntity name: {worldRegionPolygon.WorldRegion.ToString()}");
                    var worldRegion = Territory.GetTerritoryRepository.FindOne(x =>
                        x.territoryName == worldRegionPolygon.WorldRegion.ToString());
                    if (worldRegion == null)
                    {
                        worldRegion = new Territory
                        {
                            territoryName = worldRegionPolygon.WorldRegion.ToString(),
                            MinX = worldRegionPolygon.PolygonBounds.Min.x,
                            MinY = worldRegionPolygon.PolygonBounds.Min.y,
                            MinZ = worldRegionPolygon.PolygonBounds.Min.z,
                            MaxX = worldRegionPolygon.PolygonBounds.Max.x,
                            MaxY = worldRegionPolygon.PolygonBounds.Max.y,
                            MaxZ = worldRegionPolygon.PolygonBounds.Max.z,
                            CenterX = worldRegionPolygon.PolygonBounds.Center.x,
                            CenterY = worldRegionPolygon.PolygonBounds.Center.y,
                            CenterZ = worldRegionPolygon.PolygonBounds.Center.z
                        };
                        Territory.GetTerritoryRepository.Insert(worldRegion);
                        
                    }
                    // else
                    // {
                    //     worldRegion.MinX = worldRegionPolygon.PolygonBounds.Min.x;
                    //     worldRegion.MinY = worldRegionPolygon.PolygonBounds.Min.y;
                    //     worldRegion.MinZ = worldRegionPolygon.PolygonBounds.Min.z;
                    //     worldRegion.MaxX = worldRegionPolygon.PolygonBounds.Max.x;
                    //     worldRegion.MaxY = worldRegionPolygon.PolygonBounds.Max.y;
                    //     worldRegion.MaxZ = worldRegionPolygon.PolygonBounds.Max.z;
                    //     worldRegion.CenterX = worldRegionPolygon.PolygonBounds.Center.x;
                    //     worldRegion.CenterY = worldRegionPolygon.PolygonBounds.Center.y;
                    //     worldRegion.CenterZ = worldRegionPolygon.PolygonBounds.Center.z;
                    //     Territory.GetTerritoryRepository.Update(worldRegion);
                    //     Plugin.Logger?.LogWarning("Updated territory entry");
                    // }
                    //Plugin.Logger?.LogWarning($"worldRegionPolygon.PolygonBounds.Min: {worldRegionPolygon.PolygonBounds.Min.ToString()} worldRegionPolygon.PolygonBounds.Max: {worldRegionPolygon.PolygonBounds.Max.ToString()} worldRegionPolygon.PolygonBounds.Center: {worldRegionPolygon.PolygonBounds.Center.ToString()}");

                    

                    /*
                    Plugin.Logger?.LogWarning($"[worldRegionEntity][{worldRegionEntity.ToString()}]");
                    var i = 1;
                    foreach (var type in __instance.EntityManager.GetComponentTypes(worldRegionEntity))
                    {
                        Plugin.Logger?.LogWarning($"[{i++}][ComponentType][{type}]");
                    }

                    Plugin.Logger?.LogWarning("--------------------");
                    */
                    // [worldRegionEntity][Entity(1024:1)]
                    // [1][ComponentType][Parent]
                    // [2][ComponentType][LocalToParent]
                    // [3][ComponentType][LocalToWorld]
                    // [4][ComponentType][Rotation]
                    // [5][ComponentType][Translation]
                    // [6][ComponentType][WorldRegionPolygon]
                    // [7][ComponentType][WorldRegionPolygonVertex [B]]
                    // [8][ComponentType][StaticSceneTag]
                    // [9][ComponentType][SceneTag]
                    // [10][ComponentType][SceneSection]
                }
            }

            if (!__instance._CastleTerritoryQuery.IsEmpty)
            {
                // foreach (var castleTerritoryEntity in
                //          __instance._CastleTerritoryQuery.ToEntityArray(Allocator.Temp))
                // {
                    // var castleTerritory =
                    //     __instance.EntityManager.GetComponentDataAOT<CastleTerritory>(castleTerritoryEntity);
                    // MapZoneCollectionSystem has MapZoneCollection (MapZoneCollectionSystem.GetMapZoneCollection())
                    // MapZoneCollection has
                    // public readonly NativeReference<FixedList512<SpatialMapZoneData>> _Empty;
                    //
                    // public readonly NativeArray<FixedList512<SpatialMapZoneData>> _SpatialLookup;
                    //
                    // public readonly NativeHashMap<MapZoneId, SpatialMapZoneData> _MapZoneLookup; THIS ONE IS INTERESTING
                    //
                    // public readonly Nullable_Unboxed<SpatialMapZoneData> _GlobalCastleTerritory;
                    //
                    // var mapZoneCollectionSystem = VWorld.Server.GetExistingSystem<MapZoneCollectionSystem>();
                    // if (mapZoneCollectionSystem == null)
                    // {
                    //     continue;
                    // }
                    // try
                    // {
                    //     var hasZone = mapZoneCollectionSystem.GetMapZoneCollection().TryGetZone(castleTerritory.ZoneId, out var castleSpatialMapZoneData);
                    //     if (hasZone)
                    //     {
                    //         Plugin.Logger?.LogWarning(
                    //             $"[castleSpatialMapZoneData][{castleSpatialMapZoneData.ZoneFlags.ToString()}]");
                    //         Plugin.Logger?.LogWarning(
                    //             $"[castleSpatialMapZoneData][min :x{castleSpatialMapZoneData.WorldBounds.Min.x}][y{castleSpatialMapZoneData.WorldBounds.Min.y}]");
                    //         Plugin.Logger?.LogWarning(
                    //             $"[castleSpatialMapZoneData][max :x{castleSpatialMapZoneData.WorldBounds.Max.x}][y{castleSpatialMapZoneData.WorldBounds.Max.y}]");
                    //         Plugin.Logger?.LogWarning(
                    //             $"[castleSpatialMapZoneData][center: x{castleSpatialMapZoneData.WorldBounds.Center.x}][y{castleSpatialMapZoneData.WorldBounds.Center.y}]");
                    //     }
                    //
                    //     if (castleTerritory.CastleHeart == Entity.Null) continue;
                    // }
                    // catch (NullReferenceException e)
                    // {
                    //     
                    // }
                    
                    // Plugin.Logger?.LogWarning($"[CastleHeart][{castleTerritory.CastleHeart.ToString()}]");
                    // var i = 1;
                    // foreach (var type in __instance.EntityManager.GetComponentTypes(castleTerritory.CastleHeart))
                    // {
                    //     Plugin.Logger?.LogWarning($"[{i++}][ComponentType][{type}]");
                    // }
                    /*
                    Plugin.Logger?.LogWarning($"[castleTerritoryEntity][{castleTerritoryEntity.ToString()}]");
                    var i = 1;
                    foreach (var type in __instance.EntityManager.GetComponentTypes(castleTerritoryEntity))
                    {
                        Plugin.Logger?.LogWarning($"[{i++}][ComponentType][{type}]");
                    }
                    Plugin.Logger?.LogWarning("--------------------");
                    */

                    //[castleTerritoryEntity][Entity(859:1)]
                    //[1][ComponentType][CastleTerritory]
                    //[2][ComponentType][MapZoneData]
                    //[3][ComponentType][MapZonePolygonVertexElement [B]]
                    //[4][ComponentType][CastleTerritoryBlocks [B]]
                    //[5][ComponentType][CastleTerritoryTiles [B]]
                // }
                // important: WorldRegionType
            }
        }
        catch (Exception e)
        {
            Plugin.Logger?.LogError("error: " + e.Message);
            Plugin.Logger?.LogError("Stacktrace: " + e.StackTrace);
        }
    }
}