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
        public string UserId { get; set; } = null!;

        [Required]
        public int BackpackId { get; set; }
    }
}