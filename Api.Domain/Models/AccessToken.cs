using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class AccessToken : BaseEntity
    {
        [BsonElement("token")]
        [JsonProperty("token")]
        public string Token { get; set; }

        [BsonElement("userId")]
        [JsonProperty("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonElement("email")]
        [JsonProperty("email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [BsonElement("role")]
        [JsonProperty("role")]
        public string Role { get; set; }
    }
}