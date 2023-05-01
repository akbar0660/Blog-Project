using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(AppDbContext db , UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            int showCount = 3;
            IQueryable<AppUser> users = _db.AppUsers.OrderByDescending(x => x.Id).Take(showCount);
            ViewBag.PageCount = Math.Ceiling((Decimal)await _db.AppUsers.CountAsync() / showCount);
            return View( await users.ToListAsync());
        }
        public async Task<IActionResult> Details(string id)
        {
            AppUser user = await _db.AppUsers.Where(p => p.Id == id).FirstOrDefaultAsync();
            ViewBag.Role = await _userManager.GetRolesAsync(user);
            if (user == null)
            {
                return BadRequest();
            }
            return View(user);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _db.Roles.ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser user , string rolename)
        {
            bool isExist = await _db.AppUsers.AnyAsync(x=>x.Name==user.Name);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This user already exists");
                return View();
            }
            var passwordHasher = new PasswordHasher<AppUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, user.Password);
            user.Status = true;
            var result = await _userManager.CreateAsync(user);
            var role = await _roleManager.FindByNameAsync(rolename);
            await _userManager.AddToRoleAsync(user, role.Name);
            return RedirectToAction("Index");
                
        }
        public async Task<IActionResult> Activity(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            AppUser dbuser = await _db.AppUsers.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (dbuser == null)
            {
                return BadRequest();
            }
            if (dbuser.Status)  
            {
                dbuser.Status = false;
            }
            else
            {   
                dbuser.Status = true;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Pagination(int page)
        {
            int showCount = 3;
            IQueryable<AppUser> users = _db.AppUsers.OrderByDescending(x => x.Id).Skip((page - 1) * showCount).Take(showCount);
            return PartialView("_UserPagination", await users.ToListAsync());
        }
    }
}
