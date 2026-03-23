using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.Entities
{
    public class Category
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = null!;

        [InverseProperty("Category")]
        public List<Backpack> Backpacks { get; set; } = new List<Backpack>();
    }
}