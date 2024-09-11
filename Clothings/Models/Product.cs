using System;
using System.Collections.Generic;

namespace Clothings.Models
{
    public class Product
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public string ImageFileName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public virtual Category Category { get; set; } = null!;
        
    }
}
