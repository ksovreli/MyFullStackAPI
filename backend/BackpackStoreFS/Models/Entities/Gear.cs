using System.ComponentModel.DataAnnotations;

namespace BackpackStoreFS.Models.Entities
{
    public class Gear
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string SerialNumber { get; set; } = null!;

        public string Description { get; set; } = null!;

        [Required]
        [Range(0, 999999)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public string Category { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;

        public bool IsInStock => StockQuantity > 0;
    }
}