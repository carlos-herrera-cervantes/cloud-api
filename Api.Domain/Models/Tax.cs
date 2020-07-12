using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Domain.Models
{
    public class Tax : BaseEntity
    {
        public decimal Percentage { get; set; }

        public string Name { get; set; }
    }
}
