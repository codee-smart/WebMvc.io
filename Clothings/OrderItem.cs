using Clothings.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clothings
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; } // Foreign key to Order
        [ForeignKey("OrderId")]

        [Required]
        public int ProductId { get; set; } // Foreign key to Product
        [ForeignKey("ProductId")]
        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        // Navigation property for the related Order
        public Order Order { get; set; } = null!;

        // Navigation property for the related Product
        public Product Product { get; set; } = null!;
    }
}
