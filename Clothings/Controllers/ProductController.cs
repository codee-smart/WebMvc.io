using Clothings.Data;
using Clothings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Clothings.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var products = context.Products
                    .Include(p => p.Category)  // This will include the Category data
                    .OrderByDescending(p => p.Id)
                    .ToList();

            return View(products);

            //return View(products);
            //var products = context.Products.OrderByDescending(p => p.Id).ToList();
            //return View(products);
        }
        private IEnumerable<SelectListItem> GetTypesList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "veg", Value = "1" },
                new SelectListItem { Text = "Non-Veg", Value = "2" },
                new SelectListItem { Text = "None", Value = "3" }
            };
        }

        public IActionResult Create()
        {
            var categories = context.categories
        .Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

            var productDto = new ProductDto
            {
                CategoriesList = categories,
                TypesList = GetTypesList()
            };

            return View(productDto);
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
            if (!ModelState.IsValid)
            {
                productDto.CategoriesList = context.categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
                productDto.TypesList = GetTypesList();
                return View(productDto);
            }

            // Save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            // Fetch the category from the database
            var category = context.categories.Find(productDto.CategoryId);

            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid category");
                productDto.CategoriesList = context.categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
                return View(productDto);
            }
            string typeName = GetTypeName(productDto.Type);
            // Save the new product into the database
            Product product = new Product()
            {
                Name = productDto.Name,
                Type = typeName,
                Category = category,
                CategoryName = category.Name,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

        private string GetTypeName(string typeId)
        {
            return typeId switch
            {
                "1" => "veg",
                "2" => "Non-Veg",
                "3" => "None",
                _ => "Unknown"
            };
        }
        public IActionResult Edit(int id)
        {
            var product = context.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            var productDto = new ProductDto()
            {
                Name = product.Name,
                Type = product.Type,
                CategoryId = product.CategoryId, // Map CategoryId
                CategoryName = product.Category?.Name ?? "", // Map CategoryName if needed
                Price = product.Price,
                Description = product.Description,
                // Initialize the CategoriesList for the dropdown
                CategoriesList = context.categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList(),
                // Initialize the TypesList for the dropdown
                TypesList = GetTypesList()
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }



        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            if (!ModelState.IsValid)
            {
                productDto.CategoriesList = context.categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();

                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            // Fetch the new category if it changed
            var category = context.categories.Find(productDto.CategoryId);
            if (category == null)
            {
                ModelState.AddModelError("CategoryId", "Invalid category");
                productDto.CategoriesList = context.categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();

                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            // Update the image file if necessary and save changes
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                // Delete the old image
                string oldImageFullPath = Path.Combine(environment.WebRootPath, "products", product.ImageFileName);
                if (System.IO.File.Exists(oldImageFullPath))
                {
                    System.IO.File.Delete(oldImageFullPath);
                }
            }
            string typeName = GetTypeName(productDto.Type);

            // Update the product details
            product.Name = productDto.Name;
            product.Type = typeName;
            product.Category = category;
            product.CategoryName = category.Name;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }


        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Product");
            }

            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Product");
        }
    }
}
