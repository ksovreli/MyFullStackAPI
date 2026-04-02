using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.Entities
{
    public class WishlistItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [Column("backpack_id")]
        public int BackpackId { get; set; }

        [ForeignKey("BackpackId")]
        public Backpack? Backpack { get; set; }
    }
}