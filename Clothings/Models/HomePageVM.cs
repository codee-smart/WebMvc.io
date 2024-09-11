using Clothings;
using Clothings.Models;

namespace Clothings.Models
{
    public class HomePageVM
    {
        public IEnumerable<Product> ProductList { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string searchByName { get; set; }
    }
}
