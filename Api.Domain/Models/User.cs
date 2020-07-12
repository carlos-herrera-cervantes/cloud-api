using System;
using Api.Domain.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Domain.Models
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } = Roles.Employee;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}