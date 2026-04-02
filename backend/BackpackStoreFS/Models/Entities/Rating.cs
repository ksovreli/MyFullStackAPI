using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.Entities
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 5)]
        public int Value { get; set; }
        public int UserId { get; set; }

        [ForeignKey("Backpack")]
        public int BackpackId { get; set; }
        public Backpack Backpack { get; set; } = null!;
    }
}
