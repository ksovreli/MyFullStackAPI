using System.ComponentModel.DataAnnotations;

namespace BackpackStoreFS.Models.DTOs
{
    public class WishlistDto
    {
        public string UserId { get; set; } = null!;
        public int BackpackId { get; set; }
    }
}
