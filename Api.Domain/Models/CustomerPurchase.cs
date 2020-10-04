using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Domain.Models
{
    public class CustomerPurchase : BaseEntity
    {
        [Required(ErrorMessage = "PurchaseFolioRequired")]
        public string Folio { get; set; }

        [Required(ErrorMessage = "PurchaseIvaRequired")]
        public decimal? Iva { get; set; }

        [Required(ErrorMessage = "PurchaseSubtotalRequired")]
        public decimal? Subtotal { get; set; }

        [Required(ErrorMessage = "PurchaseTotalRequired")]
        public decimal? Total { get; set; }

        [Required(ErrorMessage = "PurchaseTotalLettersRequired")]
        public string TotalLetters { get; set; }

        [Required(ErrorMessage = "PurchaseEmployeeRequired")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        [Required(ErrorMessage = "PurchaseProductRequired")]
        [MinLength(1, ErrorMessage = "PurchaseProductRequired")]
        public ProductSold[] Products { get; set; }

        [Required(ErrorMessage = "PurchasePaymentRequired")]
        [MinLength(1, ErrorMessage = "PurchasePaymentRequired")]
        public Payment[] Payments { get; set; }

        [Required(ErrorMessage = "PurchaseClientRequired")]
        public Client Client { get; set; }
    }
}
