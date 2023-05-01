using Application.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryReadRepository _categoryRead;
        private readonly ICategoryWriteRepository _categoryWrite;
        public CategoryController(ICategoryWriteRepository categoryWrite,ICategoryReadRepository categoryRead)
        {
            _categoryWrite = categoryWrite;
            _categoryRead = categoryRead;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string key)
        {
            int showCount = 3;
            ViewBag.PageCount = Math.Ceiling((Decimal)await _categoryRead.CountAsync() / showCount);
            IQueryable<Category> categories = _categoryRead.GetAll().OrderByDescending(x => x.Id).Take(showCount);
            if (!string.IsNullOrEmpty(key))
            {
                ViewBag.Key = key;
                categories =_categoryRead.GetWhere(x => x.Name.ToLower().Contains(key.ToLower())).Include(p => p.Blogs);
            }
            return View(await categories.ToListAsync());
        }
        public async Task<IActionResult> Details(int id)
        {
            Category category = await _categoryRead.GetWhere(p => p.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return BadRequest();
            }
            return View(category);
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category dbCatagory = await _categoryRead.GetWhere(x => x.Id == id).FirstOrDefaultAsync();
            if (dbCatagory == null)
            {
                return BadRequest();
            }

            if (dbCatagory.Status)
            {
                dbCatagory.Status = false;
            }
            else
            {
                dbCatagory.Status = true;
            }


            await _categoryWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            bool isExist = await _categoryRead.IsExist(category.Id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }
            category.Status = true;
            await _categoryWrite.AddAsync(category);
            await _categoryWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Category dbCategory =await _categoryRead.GetWhere(x => x.Id == id).FirstOrDefaultAsync();
            if(dbCategory == null)
            {
                return BadRequest();
            }
            return View(dbCategory);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Category category,int? id)
        {
            if (id == null)
            {
                return NotFound();  
            }
            Category dbCategory = await _categoryRead.GetWhere(x => x.Id == id).FirstOrDefaultAsync();
            if (dbCategory == null)
            {
                return BadRequest();
                
            }
            //bool isExist = await _categoryRead.IsExist(category.Id);
            //if (isExist && )
            //{
            //    ModelState.AddModelError("Name", "This category already exists");
            //    return View();
            //}
            dbCategory.LastUpdatedTime = DateTime.UtcNow;
            dbCategory.Description=category.Description;
            dbCategory.Name=category.Name;
            await _categoryWrite.SaveChangesAsync();
            return RedirectToAction("Index");
            
        }
        public async Task<IActionResult> Pagination(int page=1)
        {
            int showCount = 3;
            IQueryable<Category> categories = _categoryRead.GetAll().OrderByDescending(x => x.Id).Skip((page - 1) * showCount).Take(showCount);
            return PartialView("_CategoryPagination", await categories.ToListAsync());
        }
    }
}
