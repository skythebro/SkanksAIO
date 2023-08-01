using LiteDB;

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
                    _collection?.EnsureIndex(x => x.Id);
                }
                return _collection!;
            }
        }

        public ObjectId? Id { get; set; }
        public string? territoryName { get; set; }
        public int playersInRegion { get; set; } = 0;
        public int totalClaims { get; set; } = 0;
        public int usedClaims { get; set; } = 0;
        public int freeClaims { get; set; } = 0;
       

        

        public Territory() { }

        [BsonCtor]
        public Territory(ObjectId id, string? territoryName	, int playersInRegion, int totalClaims, int usedClaims, int freeClaims)
        {
            this.Id = id;
            this.territoryName = territoryName;
            this.playersInRegion = playersInRegion;
            this.totalClaims = totalClaims;
            this.usedClaims = usedClaims;
            this.freeClaims = freeClaims;
        }
    
}