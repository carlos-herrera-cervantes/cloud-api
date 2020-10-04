using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class Client : BaseEntity
    {
        [Required(ErrorMessage = "PurchaseClientEmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string Email { get; set; }
    }
}
