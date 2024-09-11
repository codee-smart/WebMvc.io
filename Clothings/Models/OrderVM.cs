using System.ComponentModel.DataAnnotations;

namespace Clothings.Models
{
    public class OrderVM
    {
        public IEnumerable<userCart> CartItems { get; set; } = new List<userCart>();

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = "";

        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = "";
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = "";

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
    }
}
