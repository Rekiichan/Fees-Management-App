using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace FeeCollectorApplication.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [BsonElement]
        [JsonPropertyName("img")]
        public string? image { get; set; }
        [BsonElement]
        [JsonPropertyName("idCar")]
        public string idCar { get; set; }
        [BsonElement]
        [JsonPropertyName("type")]
        public string? type { get; set; }
        [BsonElement]
        [JsonPropertyName("date")]
        public string? date { get; set; }
        [BsonElement]
        [JsonPropertyName("time")]
        public string? time { get; set; }
        [BsonElement]
        [JsonPropertyName("timeM")]
        public string? timeM { get; set; }
        [BsonElement]
        [JsonPropertyName("location")]
        public string? location { get; set; }
        [BsonElement]
        [JsonPropertyName("price")]
        public Int32? price { get; set; }
        [BsonElement]
        [JsonPropertyName("pendingStatus")]
        public bool pendingStatus { get; set; }
        [BsonElement]
        [JsonPropertyName("latitude")]
        public double? latitude { get; set; }
        [BsonElement]
        [JsonPropertyName("longtitude")]
        public double? longtitude { get; set; }
    }
}
