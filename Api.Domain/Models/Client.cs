using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class Client : BaseEntity
    {
        [BsonElement("email")]
        [JsonProperty("email")]
        [Required(ErrorMessage = "PurchaseClientEmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string Email { get; set; }
    }
}
