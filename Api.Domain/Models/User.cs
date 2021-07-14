using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Api.Domain.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class User : BaseEntity
    {
        [BsonElement("firstName")]
        [JsonProperty("firstName")]
        [Required(ErrorMessage = "FirstNameRequired")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        [JsonProperty("lastName")]
        [Required(ErrorMessage = "LastNameRequired")]
        public string LastName { get; set; }

        [BsonElement("email")]
        [JsonProperty("email")]
        [Required(ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string Email { get; set; }

        [BsonElement("password")]
        [JsonProperty("password")]
        [Required(ErrorMessage = "PasswordRequired")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "InvalidPassword")]
        public string Password { get; set; }

        [BsonElement("role")]
        [JsonProperty("role")]
        [RegularExpression("Employee|StationAdmin|SuperAdmin", ErrorMessage = "InvalidRole")]
        public string Role { get; set; } = Roles.Employee;

        [BsonElement("stationId")]
        [JsonProperty("stationId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string StationId { get; set; }
    }

    public class SingleUserResponse : BaseResponse
    {
        [JsonProperty("data")]
        public User Data { get; set; }
    }

    public class ListUserResponse : ListBaseResponse
    {
        [JsonProperty("data")]
        public List<User> Data { get; set; }
    }
}