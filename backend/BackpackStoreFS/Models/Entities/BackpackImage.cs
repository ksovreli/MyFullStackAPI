using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.Entities
{
    public class BackpackImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; } = null!;

        [ForeignKey("Backpack")]
        public int BackpackId { get; set; }
        public Backpack Backpack { get; set; } = null!;
    }
}
