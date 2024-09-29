using VendaPues.Shared.Entities;
using VendaPues.Shared.Enums;

namespace VendaPues.Shared.DTOs
{
    public class GrossProfitReportDTO
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public User? User { get; set; }

        public OrderType OrderType { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public Product? Product { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public float Quantity { get; set; }

        public decimal Value => (decimal)Quantity * Price;

        public decimal Profit => (decimal)Quantity * (Price - Product!.Cost);
    }
}