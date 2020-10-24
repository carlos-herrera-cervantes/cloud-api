using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class ProductSold : BaseEntity
    {
        [BsonElement("name")]
        [JsonProperty("name")]
        [Required(ErrorMessage = "PurchaseProductNameRequired")]
        public string Name { get; set; }

        [BsonElement("description")]
        [JsonProperty("description")]
        public string Description { get; set; }

        [BsonElement("quantity")]
        [JsonProperty("quantity")]
        [Required(ErrorMessage = "PurchaseProductQuantityRequired")]
        [Range(typeof(Decimal), "1", "1000000000000000000", ErrorMessage = "PurchaseInvalidProductQuantity")]
        public int? Quantity { get; set; }

        [BsonElement("priceUnit")]
        [JsonProperty("priceUnit")]
        [Required(ErrorMessage = "PurchaseProductPriceRequired")]
        [Range(typeof(Decimal), "1", "1000000000000000000", ErrorMessage = "PurchaseInvalidProductPriceQuantity")]
        public decimal? PriceUnit { get; set; }

        [BsonElement("price")]
        [JsonProperty("price")]
        [Required(ErrorMessage = "PurchaseProductPriceTotalRequired")]
        [Range(typeof(Decimal), "1", "1000000000000000000", ErrorMessage = "PurchaseInvalidProductPriceTotalQuantity")]
        public decimal? Price { get; set; }

        [BsonElement("measurementUnit")]
        [JsonProperty("measurementUnit")]
        [Required(ErrorMessage = "PurchaseProductMeasurementUnitRequired")]
        public string MeasurementUnit { get; set; }

        [BsonElement("measurementUnitSat")]
        [JsonProperty("measurementUnitSat")]
        public string MeasurementUnitSat { get; set; }

        [BsonElement("taxes")]
        [JsonProperty("taxes")]
        public Tax[] Taxes { get; set; }
    }
}
