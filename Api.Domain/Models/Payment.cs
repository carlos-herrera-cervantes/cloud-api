namespace Api.Domain.Models
{
    public class Payment : BaseEntity
    {
        public decimal Quantity { get; set; }

        public string Key { get; set; }

        public string Description { get; set; }
    }
}
