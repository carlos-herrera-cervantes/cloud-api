namespace Api.Domain.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; }

        public decimal PriceUnit { get; set; }

        public decimal Price { get; set; }

        public string MeasurementUnit { get; set; }

        public string MeasurementUnitSat { get; set; }

        public Tax[] Taxes { get; set; }
    }
}
