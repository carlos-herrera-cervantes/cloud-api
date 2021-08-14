using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class Payment : BaseEntity
    {
        [BsonElement("quantity")]
        [JsonProperty("quantity")]
        [Required(ErrorMessage = "PurchasePaymentQuantityRequired")]
        public decimal? Quantity { get; set; }

        [BsonElement("key")]
        [JsonProperty("key")]
        [Required(ErrorMessage = "PaymentKeyRequired")]
        public string Key { get; set; }

        [BsonElement("description")]
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
