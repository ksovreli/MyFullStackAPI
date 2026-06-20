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

        [Required]
        [Column("price", TypeName = "decimal(5,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(1000)]
        [Column("description")]
        public string Description { get; set; } = null!;

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

        public List<BackpackImage> Images { get; set; } = new List<BackpackImage>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Rating> Ratings { get; set; } = new List<Rating>();

        public Backpack()
        {
            Images = new List<BackpackImage>();
            Reviews = new List<Review>();
            Ratings = new List<Rating>();
        }

        public Backpack(BackpackCreateDto dto)
        {
            Name = dto.Name;
            Price = dto.Price;
            Description = dto.Description;
            Quantity = dto.Quantity;
            SalePrice = dto.SalePrice;
            CategoryId = dto.CategoryId;
            IsNew = dto.IsNew;

            if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                Images.Add(new BackpackImage { Url = dto.ImageUrl });
            }

            if (dto.Rating > 0)
            {
                Ratings.Add(new Rating
                {
                    Value = (int)Math.Round(dto.Rating),
                    UserId = 0
                });
            }
        }
    }
}