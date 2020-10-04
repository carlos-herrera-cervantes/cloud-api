using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class Product : BaseEntity
    {
        [Required(ErrorMessage = "FirstNameRequired")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ProductDescriptionRequired")]
        public string Description { get; set; }

        [Required(ErrorMessage = "ProductPriceRequired")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "ProductPricePublicRequired")]
        public decimal PricePublic { get; set; }
    }
}