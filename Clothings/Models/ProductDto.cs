using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

public class ProductDto
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Type { get; set; } = ""; 
    public IEnumerable<SelectListItem> TypesList { get; set; } = new List<SelectListItem>();
    

    [Required]
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public IEnumerable<SelectListItem> CategoriesList { get; set; } = new List<SelectListItem>();

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Description { get; set; } = "";

    public IFormFile? ImageFile { get; set; }

    public string ImageUrl { get; set; } = "";

    public DateTime AddedAt { get; set; }
}
