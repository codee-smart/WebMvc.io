using System.ComponentModel.DataAnnotations;

namespace Clothings
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!; // Store user ID or email

        [Required]
        public string CustomerName { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        // Navigation property for related order items
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
