using System;
using System.Collections.Generic;
using Unity.Entities;

namespace SkanksAIO.Utils;

public class Structs
{
    public struct PlayerGroup
    {
        public int AllyCount { get; set; }
        public Dictionary<Entity, Entity> Allies { get; set; }
        public DateTime TimeStamp { get; set; }

        public PlayerGroup(int allyCount = 0, Dictionary<Entity, Entity> allies = default, DateTime timeStamp = default)
        {
            AllyCount = allyCount;
            Allies = allies;
            TimeStamp = timeStamp;
        }
    }
}