using System.ComponentModel.DataAnnotations;
using Api.Domain.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Domain.Models
{
    public class User : BaseEntity
    {
        [Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "InvalidPassword")]
        public string Password { get; set; }

        public string Role { get; set; } = Roles.Employee;

        [BsonRepresentation(BsonType.ObjectId)]
        public string StationId { get; set; }
    }
}