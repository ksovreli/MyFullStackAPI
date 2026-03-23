using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.Entities
{
    public class User : IdentityUser<int>
    {
        [Column("role")]
        public string Role { get; set; } = "User";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}