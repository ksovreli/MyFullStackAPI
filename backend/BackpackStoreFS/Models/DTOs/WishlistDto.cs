using System.ComponentModel.DataAnnotations;

namespace BackpackStoreFS.Models.DTOs
{
    public class WishlistDto
    {
        public int UserId { get; set; }
        public int BackpackId { get; set; }
    }
}
