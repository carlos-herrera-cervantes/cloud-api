using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Domain.Models
{
    public class Client : BaseEntity
    {
        public string Email { get; set; }
    }
}
