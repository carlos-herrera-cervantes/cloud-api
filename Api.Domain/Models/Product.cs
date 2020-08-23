namespace Api.Domain.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; } = 0m;

        public decimal PricePublic { get; set; } = 0m;
    }
}