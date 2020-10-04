using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class Credentials
    {
        [Required(ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; }
    }
}