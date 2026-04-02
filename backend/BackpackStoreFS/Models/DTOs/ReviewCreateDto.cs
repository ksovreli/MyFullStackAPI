using System.ComponentModel.DataAnnotations;

namespace BackpackStoreFS.Models.DTOs
{
    public class ReviewCreateDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        [Required]
        public int BackpackId { get; set; }
    }

    public class ReviewReadDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public int BackpackId { get; set; }
    }
}