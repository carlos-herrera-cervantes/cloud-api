using System.ComponentModel.DataAnnotations;

namespace Api.Domain.Models
{
    public class Client : BaseEntity
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
