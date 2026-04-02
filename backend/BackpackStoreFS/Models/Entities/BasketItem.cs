using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.Entities
{
    public class BasketItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        public int BackpackId { get; set; }

        public Backpack? Backpack { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
