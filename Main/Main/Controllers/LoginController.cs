using Domain.Entities;
using Main.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Main.Controllers
{
    public class LoginController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public LoginController(RoleManager<IdentityRole> roleManager , UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index(LoginVM login)
        {
            var password = login.Password.ToLower();
            var email = login.Email.ToString().ToLower();
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "Email is wrong");
                return View("Index", login);
            };
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, password, true, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Your account has been blocked for 5 minutes");
                return View(login);
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Password", "Password is wrong");
                return View(login);
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else if (roles.Contains("Writer"))
                {
                    return RedirectToAction("Index", "Blog");
                }
                else if (roles.Contains("Customer"))
                {
                    HttpContext.Session.SetString("Username",user.UserName);
                    return RedirectToAction("Index", "Comment", new { area = "Comment" });
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
        }
        //public async Task<IActionResult> Index(LoginVM model, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
        //        if (result.Succeeded)
        //        {
        //            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //            {
        //                return Redirect(returnUrl);
        //            }
        //            else
        //            {
        //                var user = await _userManager.FindByEmailAsync(model.Email);
        //                var roles = await _userManager.GetRolesAsync(user);
        //                if (roles.Contains("Admin"))
        //                {
        //                    return RedirectToAction("Index", "Dashboard");
        //                }
        //                else if (roles.Contains("Writer"))
        //                {
        //                    return RedirectToAction("Index", "Blog");
        //                }
        //                //else if (roles.Contains("Customer"))
        //                //{
        //                //    return RedirectToAction("CustomerPage", "Customer");
        //                //}
        //                else
        //                {
        //                    return RedirectToAction("Index", "Dashboard");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //        }
        //    }
        //    return View(model);
        //}

        //public async Task<IActionResult> Index(LoginVM login)
        //{
        //    var password = login.Password.ToLower();
        //    var email = login.Email.ToString().ToLower();
        //    AppUser user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError("Email", "Email is wrong");
        //        return View("Index",login);
        //    };
        //    Microsoft.AspNetCore.Identity.SignInResult result =  await _signInManager.PasswordSignInAsync(user, password ,true,true);
        //    if (result.IsLockedOut)
        //    {
        //        ModelState.AddModelError("", "Your account has been blocked for 5 minutes");
        //        return View(login);
        //    }
        //    if (!result.Succeeded)
        //    {
        //        ModelState.AddModelError("", "Email or password is wrong");
        //        return View(login);
        //    }
        //    var roles = await _userManager.GetRolesAsync(user);
        //    if (roles.Contains("Admin"))
        //    {
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    else if (roles.Contains("Writer"))
        //    {
        //        return RedirectToAction("Index", "Blog");
        //    }
        //    //else if (roles.Contains("Customer"))
        //    //{
        //    //    return RedirectToAction("CustomerPage", "Customer");
        //    //}
        //    else
        //    {
        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //}
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
        // public async Task CreateRoles()
        //{
        //    if (!await _roleManager.RoleExistsAsync(Helper.Roles.Admin.ToString()))  
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = Helper.Roles.Admin.ToString() });
        //    }
        //    if (!await _roleManager.RoleExistsAsync(Helper.Roles.Writer.ToString()))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = Helper.Roles.Writer.ToString() });
        //    }
        //    if (!await _roleManager.RoleExistsAsync(Helper.Roles.Customer.ToString()))
        //    {
        //        await _roleManager.CreateAsync(new IdentityRole { Name = Helper.Roles.Customer.ToString() });
        //    }
        }
    }
