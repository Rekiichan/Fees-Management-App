using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FeeCollectorApplication.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [BsonElement]
        public string? image { get; set; }
        [BsonElement]
        public string idCar { get; set; }
        [BsonElement]
        public string? type { get; set; }
        [BsonElement]
        public string? date { get; set; }
        [BsonElement]
        public string? time { get; set; }
        [BsonElement]
        public string? timeM { get; set; }
        [BsonElement]
        public string? location { get; set; }
        [BsonElement]
        public Int32? price { get; set; }
        [BsonElement]
        public bool pendingStatus { get; set; }
        [BsonElement]
        public double? latitude { get; set; }
        [BsonElement]
        public double? longtitude { get; set; }
    }
}
