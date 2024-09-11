using Clothings.Data;
using Clothings.Models;
using Clothings.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothings.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public CartController(ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult CartIndex()
        {
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                CartIndexVM cartIndexVM = new CartIndexVM()
                {
                    productList = _db.userCarts.Include(u => u.product).Where(u => u.userId.Contains(userId)).ToList(),
                };
                var count = _db.userCarts.Where(u => userId.Contains(userId)).Count();
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);

                return View(cartIndexVM);
            }
            return View();
        }
       
        
        public async Task<IActionResult> AddToCart(int productId, string? returnUrl)
        {
            var productAddToCart = await _db.Products.FirstOrDefaultAsync(u => u.Id == productId);
            var CheckIfUserSignedInOrNot = _signInManager.IsSignedIn(User);
            if (CheckIfUserSignedInOrNot)
            {
                var user = _userManager.GetUserId(User);
                if (user != null)
                {
                    //Check if the signed user has any cart or not
                    var getTheCartIfAnyExistForTheUser = await _db.userCarts.Where(u => u.userId.Contains(user)).ToListAsync();
                    if (getTheCartIfAnyExistForTheUser.Count > 0)
                    {
                        //check if the item is already in the cart or not
                        var getTheQuantity = getTheCartIfAnyExistForTheUser.FirstOrDefault(p => p.ProductId == productId);
                        if (getTheQuantity != null)
                        {
                            getTheQuantity.Quantity = getTheQuantity.Quantity + 1;
                            _db.userCarts.Update(getTheQuantity);
                        }
                        else
                        {
                            userCart newItemToCart = new userCart
                            {
                                ProductId = productId,
                                userId = user,
                                Quantity = 1,
                            };
                            await _db.userCarts.AddAsync(newItemToCart);
                        }
                    }
                    else
                    {
                        //user has no cart. Adding a brand new cart for the user
                        userCart newItemToCart = new userCart
                        {
                            ProductId = productId,
                            userId = user,
                            Quantity = 1,
                        };
                        await _db.userCarts.AddAsync(newItemToCart);
                    }
                    await _db.SaveChangesAsync();
                }
            }
            if (returnUrl != null)
            {
                return RedirectToAction("CartIndex", "Cart");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult MinusAnItem(int productId)
        {
            var itemToMinus = _db.userCarts.FirstOrDefault(u => u.ProductId == productId);
            if (itemToMinus != null)
            {
                if (itemToMinus.Quantity - 1 == 0)
                {
                    _db.userCarts.Remove(itemToMinus);
                }
                else
                {
                    itemToMinus.Quantity -= 1;
                    _db.userCarts.Update(itemToMinus);
                }
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(CartIndex));
        }

        public IActionResult DeleteAnItem(int productId)
        {
            var itemToRemove = _db.userCarts.FirstOrDefault(u => u.ProductId == productId);
            if (itemToRemove != null)
            {
                _db.userCarts.Remove(itemToRemove);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(CartIndex));
        }

    }
}
