using System.ComponentModel.DataAnnotations;

namespace BackpackStoreFS.Models.DTOs
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
