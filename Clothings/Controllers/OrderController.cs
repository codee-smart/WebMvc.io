using Clothings.Data;
using Clothings.Models;
using Clothings.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace Clothings.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PdfGenerator _pdfGenerator;

        public OrderController(ApplicationDbContext db, UserManager<IdentityUser> userManager, PdfGenerator pdfGenerator)
        {
            _db = db;
            _userManager = userManager;
            _pdfGenerator = pdfGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetailPreview()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _db.userCarts.Include(u => u.product).Where(u => u.userId == userId).ToListAsync();

            var orderViewModel = new OrderVM
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(item => item.Quantity * item.product.Price)
            };

            return View(orderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OrderDetailPreview(OrderVM model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cartItems = await _db.userCarts.Include(u => u.product).Where(u => u.userId == userId).ToListAsync();

                // Calculate total amount
                var totalAmount = cartItems.Sum(item => item.Quantity * item.product.Price);

                // Create a new order
                var order = new Order
                {
                    CustomerId = userId,
                    CustomerName = model.CustomerName,
                    Email = model.Email,
                    ShippingAddress = model.ShippingAddress,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync(); // Save the order first to generate the OrderId

                // Create order items
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = (int)cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.product.Price
                    };
                    _db.OrderItems.Add(orderItem);
                }

                await _db.SaveChangesAsync(); // Save all order items

                // Clear the user's cart
                _db.userCarts.RemoveRange(cartItems);
                await _db.SaveChangesAsync();

                // Generate the PDF using QuestPDF
                var pdfBytes = _pdfGenerator.GeneratePdfFromHtml(model.CustomerName, model.Email, model.ShippingAddress, cartItems, order.TotalAmount);

                // Generate the filename with the current date and time
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var filename = $"Invoice_{timestamp}.pdf";

                // Return PDF as a download
                return File(pdfBytes, "application/pdf", filename);
            }

            return View(model);
        }




        private string GenerateHtmlInvoice(OrderVM model, List<userCart> cartItems)
        {
            var sb = new StringBuilder();

            sb.Append("<html><head>");
            sb.Append("<style>");
            sb.Append("body { font-family: Arial, sans-serif; }");
            sb.Append("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            sb.Append("th, td { padding: 8px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.Append("th { background-color: #f2f2f2; }");
            sb.Append("h1, h3, h4 { text-align: center; margin-top: 0; }");
            sb.Append("hr { border: 0; border-top: 1px solid #ddd; margin: 20px 0; }");
            sb.Append("</style>");
            sb.Append("</head><body>");

            sb.Append("<h1>Invoice</h1>");
            sb.Append("<hr>");

            sb.Append("<h3>Customer Details</h3>");
            sb.Append($"<p><strong>Customer Name:</strong> {model.CustomerName}</p>");
            sb.Append($"<p><strong>Email:</strong> {model.Email}</p>");
            sb.Append($"<p><strong>Shipping Address:</strong> {model.ShippingAddress}</p>");
            sb.Append("<hr>"); // Line separator after customer details

            sb.Append("<h3>Order Items</h3>");
            sb.Append("<table>");
            sb.Append("<tr><th>Product</th><th>Quantity</th><th>Unit Price</th><th>Total</th></tr>");
            foreach (var item in cartItems)
            {
                var totalPrice = item.Quantity * item.product.Price;
                sb.Append($"<tr><td>{item.product.Name}</td><td>{item.Quantity}</td><td>{item.product.Price:C}</td><td>{totalPrice:C}</td></tr>");
            }
            sb.Append("</table>");

            sb.Append($"<h4>Total Amount: {model.TotalAmount:C}</h4>");

            sb.Append("</body></html>");

            return sb.ToString();
        }


        public IActionResult OrderSuccess()
        {
            return View();
        }
    }
}
