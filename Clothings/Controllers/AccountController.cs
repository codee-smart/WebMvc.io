using Clothings.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Clothings.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginVM vm = new LoginVM();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            var user = await _userManager.Users.Where(u => u.Email == login.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                login.LoginStatus = "User not found.";
                return View(login);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, login.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                login.LoginStatus = "Your account has been locked out.";
            }
            else if (result.IsNotAllowed)
            {
                login.LoginStatus = "Login not allowed. Please confirm your email.";
            }
            else
            {
                login.LoginStatus = "Invalid login attempt.";
            }

            return View(login);
        }





        //public IActionResult Register()
        //{
        //    RegisterVM rvm = new RegisterVM();
        //    return View(rvm);
        //}
        //[HttpPost]

        //public async Task<IActionResult> Register(RegisterVM register)
        //{
        //    // Create a new ApplicationUser object
        //    var user = new ApplicationUser
        //    {
        //        FirstName = register.applicationUser.FirstName,
        //        LastName = register.applicationUser.LastName,
        //        Email = register.Email,
        //        UserName = register.UserName,
        //        Address = register.applicationUser.Address,
        //        City = register.applicationUser.City,
        //    };

        //    // Attempt to create the user with the specified password
        //    var Registration = await _userManager.CreateAsync(user, register.Password);

        //    // Check if the registration succeeded
        //    if (Registration.Succeeded)
        //    {
        //        // Sign in the user if registration was successful
        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //        register.StatusMessage = "Registered Successfully";

        //        // Redirect to the Home page
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        // Log errors if registration failed
        //        foreach (var error in Registration.Errors)
        //        {
        //            ModelState.AddModelError("", error.Description);
        //        }

        //        register.StatusMessage = "Registration was unsuccessful";
        //    }

        //    // Return the view with the current RegisterVM object
        //    return View(register);
        //}


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}