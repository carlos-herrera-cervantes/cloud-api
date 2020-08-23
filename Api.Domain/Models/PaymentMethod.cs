using System;

namespace Api.Domain.Models
{
    public class PaymentMethod : BaseEntity
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Status { get; set; } = true;
    }
}