using Clothings.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Clothings
{
    public class userCart
    {
        [Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product product { get; set; }
        public string userId { get; set; }
        [ForeignKey("userId")]
        public ApplicationUser ApplicationUser { get; set; }
        [Range(1, 100, ErrorMessage = "1 through 100")]
        public int Quantity { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}
