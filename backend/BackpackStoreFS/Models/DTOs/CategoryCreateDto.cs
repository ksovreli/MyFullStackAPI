using System.ComponentModel.DataAnnotations;

namespace BackpackStoreFS.Models.DTOs
{
    public class CategoryCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
    }
    public class CategoryReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
