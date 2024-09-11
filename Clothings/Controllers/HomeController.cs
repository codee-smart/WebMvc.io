using Clothings.Data;
using Clothings.Models;
using Clothings.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Clothings.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public IActionResult Index(string? searchByName,string? searchByCategory)
        {
            var claim = _signInManager.IsSignedIn(User);
            if (claim)
            {
                var userId = _userManager.GetUserId(User);
                var count = _db.userCarts.Where(u => userId.Contains(userId)).Count();
                HttpContext.Session.SetInt32(cartCount.sessionCount, count);
            }

            HomePageVM vm=new HomePageVM();
            if(searchByName != null)
            {
                vm.ProductList = _db.Products.Where(Name=>EF.Functions.Like(Name.Name, $"%{searchByName}%")).ToList();
                vm.Categories = _db.categories.ToList();
            }
            else if (searchByCategory !=null)
            {
                var searchByCategoryName = _db.categories.FirstOrDefault(u => u.Name == searchByCategory);
                vm.ProductList=_db.Products.Where(u=>u.CategoryId==searchByCategoryName.Id).ToList();
                vm.Categories = _db.categories.Where(u => u.Name.Contains(searchByCategory));
            }
            else
            {
                vm.ProductList = _db.Products.ToList();
                vm.Categories = _db.categories.ToList();
            }
            return View(vm);
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
