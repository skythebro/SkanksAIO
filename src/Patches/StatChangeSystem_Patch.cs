using HarmonyLib;
using ProjectM;
using ProjectM.CastleBuilding;
using Unity.Entities;
using System;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;
using SkanksAIO.Models;
using SkanksAIO.Utils;
using SkanksAIO.Utils.Config;

namespace SkanksAIO.Patches
{
    [HarmonyPatch(typeof(StatChangeSystem), nameof(StatChangeSystem.ApplyHealthChangeToEntity))]
    public class StatChangeSystem_Patch
    {
        private static void Prefix(StatChangeSystem __instance, ref StatChangeEvent statChange)
        {
            if (!Settings.DisableOfflineRaiding.Value) return;

            if (!statChange.HasFlag(StatChangeFlag.AnnounceCastleAttack))
            {
                return;
            }
            
            if (!__instance.EntityManager.HasComponent<CastleHeartConnection>(statChange.Entity)) return;
            
            var heartEntity = __instance.EntityManager.GetComponentData<CastleHeartConnection>(statChange.Entity).CastleHeartEntity._Entity;

            if (!__instance.EntityManager.HasComponent<CastleHeart>(heartEntity)) return;
            
            var castleHeart = __instance.EntityManager.GetComponentData<CastleHeart>(heartEntity);

            if (!castleHeart.State.HasFlag(CastleHeartState.IsProcessing)) return;

            if (!Cache.CastleHeartOwnerCache.TryGetValue(heartEntity, out Entity userEntity))
            {
                userEntity = __instance.EntityManager.GetComponentData<UserOwner>(heartEntity).Owner._Entity;
            }

            if (__instance.EntityManager.TryGetComponentData(userEntity, out User userData))
            {
                var playerData = Player.GetPlayerRepository.FindOne(x => x.PlatformId == userData.PlatformId);
                if (!playerData.IsConnected)
                {
                    if (Settings.FactorAllies.Value)
                    {
                        var playerAllies = GetAllies(playerData.CharEntity);
                        if (playerAllies.AllyCount > 0)
                        {
                            foreach (var ally in playerAllies.Allies)
                            {
                                var allyData = Player.GetPlayerRepository.FindOne(x => x.UserEntity == ally.Key);
                                if (allyData.IsConnected) return;
                            }
                        }
                    }

                    statChange.Change = 0;
                    statChange.OriginalChange = 0;
                }
            }
        }

        private static Structs.PlayerGroup GetAllies(Entity characterEntity)
        {
            if (Cache.AlliesCache.TryGetValue(characterEntity, out var playerGroup))
            {
                TimeSpan CacheAge = DateTime.Now - playerGroup.TimeStamp;
                if (CacheAge.TotalSeconds > Settings.MaxAllyCacheAge.Value) goto UpdateCache;
                goto ReturnResult;
            }

            UpdateCache:
            int allyCount = UserUtils.GetAllies(characterEntity, out var Group);
            playerGroup = new Structs.PlayerGroup()
            {
                AllyCount = allyCount,
                Allies = Group,
                TimeStamp = DateTime.Now
            };
            Cache.AlliesCache[characterEntity] = playerGroup;

            ReturnResult:
            return playerGroup;
        }
    }
}
