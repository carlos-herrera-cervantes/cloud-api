using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class Payment : BaseEntity
    {
        [Required(ErrorMessage = "PurchasePaymentQuantityRequired")]
        [Range(typeof(Decimal), "1", "1000000000000000000", ErrorMessage = "PurchaseInvalidPaymentQuantity")]
        public decimal? Quantity { get; set; }

        [Required(ErrorMessage = "PaymentKeyRequired")]
        public string Key { get; set; }

        public string Description { get; set; }
    }
}
