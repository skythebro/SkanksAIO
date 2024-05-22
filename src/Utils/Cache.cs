using Il2CppSystem.Collections.Generic;
using SkanksAIO.Models;
using Unity.Entities;

namespace SkanksAIO.Utils;

public static class Cache
{
    public static Dictionary<Entity, Entity> CastleHeartOwnerCache = new();
    public static Dictionary<Entity, Structs.PlayerGroup> AlliesCache = new();
}