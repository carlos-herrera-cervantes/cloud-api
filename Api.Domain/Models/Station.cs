using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class Station : BaseEntity
    {
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string StationKey { get; set; }

        public bool Active { get; set; } = true;

        public string Street { get; set; }

        public string Outside { get; set; }

        public string ZipCode { get; set; }

        public string State { get; set; }

        public string Municipality { get; set; }
    }
}
