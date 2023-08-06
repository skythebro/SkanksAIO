using LiteDB;
using Unity.Mathematics;
using Unity.Physics;

namespace SkanksAIO.Models;

public class Territory
{
    
        private static ILiteCollection<Territory>? _collection;

        public static ILiteCollection<Territory> GetTerritoryRepository
        {
            get
            {
                if (_collection == null)
                {
                    _collection = Plugin.Database?.GetCollection<Territory>();
                    _collection?.EnsureIndex(x => x.territoryName);
                }
                return _collection!;
            }
        }

        public ObjectId? Id { get; set; }
        public string? territoryName { get; set; }
    
        public int playersInRegion { get; set; } = 0;
        private int _totalClaims = 0;
        private int _usedClaims = 0;

        public int totalClaims
        {
            get => _totalClaims;
            set
            {
                _totalClaims = value;
                CalculateFreeClaims();
            }
        }

        public int usedClaims
        {
            get => _usedClaims;
            set
            {
                _usedClaims = value;
                CalculateFreeClaims();
            }
        }

        public int freeClaims { get; private set; } = 0;

        private void CalculateFreeClaims()
        {
            freeClaims = totalClaims - usedClaims;
        }
    
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MinZ { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }
        public float MaxZ { get; set; }
        public float CenterX { get; set; }
        public float CenterY { get; set; }
        public float CenterZ { get; set; }

        public Territory() { }

        [BsonCtor]
        public Territory(ObjectId id, string? territoryName, int playersInRegion, int totalClaims, int usedClaims, float3 min, float3 max, float3 center)
        {
            this.Id = id;
            this.territoryName = territoryName;
            this.playersInRegion = playersInRegion;
            this.totalClaims = totalClaims;
            this.usedClaims = usedClaims;
            MinX = min.x;
            MinY = min.y;
            MinZ = min.z;
            MaxX = max.x;
            MaxY = max.y;
            MaxZ = max.z;
            CenterX = center.x;
            CenterY = center.y;
            CenterZ = center.z;
            
        }
    
}