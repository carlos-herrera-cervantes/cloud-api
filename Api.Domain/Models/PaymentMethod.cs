using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class PaymentMethod : BaseEntity
    {
        [BsonElement("key")]
        [JsonProperty("key")]
        [Required(ErrorMessage = "PaymentKeyRequired")]
        public string Key { get; set; }

        [BsonElement("name")]
        [JsonProperty("name")]
        [Required(ErrorMessage = "FirstNameRequired")]
        public string Name { get; set; }

        [BsonElement("description")]
        [JsonProperty("description")]
        [Required(ErrorMessage = "ProductDescriptionRequired")]
        public string Description { get; set; }

        [BsonElement("status")]
        [JsonProperty("status")]
        public bool Status { get; set; } = true;
    }

    public class SinglePaymentMethodResponse : BaseResponse
    {
        [JsonProperty("data")]
        public PaymentMethod Data { get; set; }
    }

    public class ListPaymentMethodResponse : ListBaseResponse
    {
        [JsonProperty("data")]
        public IEnumerable<PaymentMethod> Data { get; set; }
    }
}