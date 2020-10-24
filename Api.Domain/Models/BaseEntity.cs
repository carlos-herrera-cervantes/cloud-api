using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class BaseEntity
    {
        [BsonElement("_id")]
        [JsonProperty("_id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("createdAt")]
        [JsonProperty("createdAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        [JsonProperty("updatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
