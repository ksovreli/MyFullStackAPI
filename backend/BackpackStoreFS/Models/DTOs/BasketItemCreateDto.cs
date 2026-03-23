using BackpackStoreFS.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.DTOs
{
    public class BasketItemCreateDto
    {
        public string UserId { get; set; } = null!;
        public int BackpackId { get; set; }
        public int Quantity { get; set; }
    }

    public class BasketItemReadDto
    {
        public int Id { get; set; }
        public int BackpackId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string Image { get; set; } = null!;
        public int Quantity { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
