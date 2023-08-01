using System;
using LiteDB;

namespace SkanksAIO.Models
{
    public class Player
    {
        private static ILiteCollection<Player>? _collection;

        public static ILiteCollection<Player> GetPlayerRepository
        {
            get
            {
                if (_collection == null)
                {
                    _collection = Plugin.Database?.GetCollection<Player>();
                    _collection?.EnsureIndex(x => x.PlatformId);
                }
                return _collection!;
            }
        }

        public ObjectId? Id { get; set; }
        public ulong PlatformId { get; set; }
        public string? CharacterName { get; set; }
        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;
        public int ELO { get; set; } = 1000;

        public double KD
        {
            get
            {
                if (Deaths == 0)
                {
                    return (double)Kills;
                }
                else
                {
                    var kd = (double)Kills / (double)Deaths;
                    return Math.Round(kd, 2);
                }
            }
        }

        public Player() { }

        [BsonCtor]
        public Player(ObjectId _id, ulong PlatformId, string? CharacterName, int Kills, int Deaths, int ELO)
        {
            this.Id = _id;
            this.PlatformId = PlatformId;
            this.CharacterName = CharacterName;
            this.Kills = Kills;
            this.Deaths = Deaths;
            this.ELO = ELO;
        }
    }
}