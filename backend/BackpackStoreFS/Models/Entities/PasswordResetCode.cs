using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackpackStoreFS.Models.Entities
{
    public class PasswordResetCode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [ForeignKey("Email")]
        public User? User { get; set; }

        [Required]
        public string Code { get; set; } = null!;

        public DateTime ExpiryTime { get; set; }
    }
}