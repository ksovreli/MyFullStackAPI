using System.ComponentModel.DataAnnotations;

namespace BackpackStoreFS.Models.DTOs
{
    public class CheckoutRequest
    {
        [Required(ErrorMessage = "Shipping address is required.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Please provide a valid full address.")]
        public string ShippingAddress { get; set; } = null!;
    }
}
