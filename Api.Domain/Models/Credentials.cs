using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

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

    public class SuccessAuth : BaseResponse
    {
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}