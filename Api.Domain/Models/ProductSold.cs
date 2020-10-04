using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class ProductSold : BaseEntity
    {
        [Required(ErrorMessage = "PurchaseProductNameRequired")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "PurchaseProductQuantityRequired")]
        [Range(typeof(Decimal), "1", "1000000000000000000", ErrorMessage = "PurchaseInvalidProductQuantity")]
        public int? Quantity { get; set; }

        [Required(ErrorMessage = "PurchaseProductPriceRequired")]
        [Range(typeof(Decimal), "1", "1000000000000000000", ErrorMessage = "PurchaseInvalidProductPriceQuantity")]
        public decimal? PriceUnit { get; set; }

        [Required(ErrorMessage = "PurchaseProductPriceTotalRequired")]
        [Range(typeof(Decimal), "1", "1000000000000000000", ErrorMessage = "PurchaseInvalidProductPriceTotalQuantity")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "PurchaseProductMeasurementUnitRequired")]
        public string MeasurementUnit { get; set; }

        public string MeasurementUnitSat { get; set; }

        public Tax[] Taxes { get; set; }
    }
}
