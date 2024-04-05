using Final_Project.Models;
using Final_Project.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM cred)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(cred.Email);
                if(user != null)
                {
                    bool found = await _userManager.CheckPasswordAsync(user, cred.Password);
                    if(found)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: cred.IsPresistent);
                        if (await _userManager.IsInRoleAsync(user, "Trader"))
                        {
                            return RedirectToAction("Home", "Trader"); // Redirect to Admin controller's Index action
                        }
                        else if (await _userManager.IsInRoleAsync(user,"Representative"))
                        {
                            return RedirectToAction("Home", "Representative");
                        }
                        else if(await _userManager.IsInRoleAsync(user,"SuperAdmin"))
                        {
                            
                        return RedirectToAction("Home", "Employee");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                ModelState.AddModelError("", "Invalid Email or password");
            }
            return View(cred);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Login");
        }
    }
}
