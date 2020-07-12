using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Api.Domain.Models
{
    public class CustomerPurchase : BaseEntity
    {
        public string Folio { get; set; }

        public decimal Iva { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Total { get; set; }

        public string TotalLetters { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Product[] Products { get; set; }

        public Payment[] Payments { get; set; }

        public Client Client { get; set; }
    }
}
