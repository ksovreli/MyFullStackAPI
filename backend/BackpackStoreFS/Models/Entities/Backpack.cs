using BackpackStoreFS.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.Entities
{
    public class Backpack
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("image")]
        public string Image { get; set; } = null!;

        [Required]
        [Column("price", TypeName = "decimal(5,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column("quantity")]
        [Range(0, 100)]
        public int Quantity { get; set; }

        [Column("sale_price", TypeName = "decimal(5,2)")]
        public decimal? SalePrice { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        public Category? Category { get; set; }

        public bool IsNew { get; set; }

        [Column("rating", TypeName = "decimal(2,1)")]
        public decimal Rating { get; set; }

        public Backpack()
        {
            
        }

        public Backpack(BackpackCreateDto dto)
        {
            Name = dto.Name;
            Image = dto.ImageUrl;
            Price = dto.Price;
            Quantity = dto.Quantity;
            SalePrice = dto.SalePrice;
            CategoryId = dto.CategoryId;
            IsNew = dto.IsNew;
            Rating = dto.Rating;
        }
    }
}