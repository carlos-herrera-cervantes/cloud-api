using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class PaymentMethod : BaseEntity
    {
        [Required(ErrorMessage = "PaymentKeyRequired")]
        public string Key { get; set; }

        [Required(ErrorMessage = "FirstNameRequired")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ProductDescriptionRequired")]
        public string Description { get; set; }

        public bool Status { get; set; } = true;
    }
}