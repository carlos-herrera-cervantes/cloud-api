using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class Station : BaseEntity
    {
        [Required(ErrorMessage = "StationNameRequired")]
        public string Name { get; set; }

        [Required(ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "StationKeyRequired")]
        public string StationKey { get; set; }

        public bool Active { get; set; } = true;

        public string Street { get; set; }

        public string Outside { get; set; }

        public string ZipCode { get; set; }

        [Required(ErrorMessage = "StationStateRequired")]
        public string State { get; set; }

        [Required(ErrorMessage = "StationMunicipality")]
        public string Municipality { get; set; }
    }
}
