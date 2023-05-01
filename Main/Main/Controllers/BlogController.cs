using Application.Repositories;
using Domain.Entities;
using Main.Helpers;
using Main.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogReadRepository _blogRead;
        private readonly IBlogWriteRepository _blogWrite;
        private readonly IWebHostEnvironment _env;
        private readonly ICategoryReadRepository _categoryRead;
        private readonly IWriterReadRepository _writerRead;
        private readonly AppDbContext _db;
        public BlogController(IBlogReadRepository blogRead, IBlogWriteRepository blogWrite, IWebHostEnvironment env, ICategoryReadRepository categoryRead, IWriterReadRepository readRepository,AppDbContext db)
        {
            _blogRead = blogRead;
            _blogWrite = blogWrite;
            _env = env;
            _categoryRead = categoryRead;
            _writerRead = readRepository;
            _db = db;
        }
        [Authorize(Roles = "Admin , Writer")]
        public async Task<IActionResult> Index()
        {
            int showCount = 3;
            IQueryable<Blog> blogs = _blogRead.GetAll().OrderByDescending(x=>x.Id).Take(showCount).Include(p => p.Category).Include(x => x.Writer);
            ViewBag.PageCount = Math.Ceiling((Decimal)await _blogRead.CountAsync() / showCount);
            List<string> years = await _blogRead.GetAll().Select(b => b.CreatedDate.Year.ToString()).Distinct().ToListAsync();
            var categories = await _categoryRead.GetAll().Distinct().ToListAsync();
            ViewBag.Years = years;
            ViewBag.Categories = categories;
            return View(await blogs.ToListAsync());
        }
        [Authorize(Roles = "Admin , Writer")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryRead.GetAll().ToListAsync();
            ViewBag.Writers = await _writerRead.GetAll().ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            ViewBag.BlogsWithCategories = await _blogRead.GetAll().Include(p => p.Category).ToListAsync();
            bool isExist = await _blogRead.IsExist(blog.Id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This blog already exists");
                return View();
            }
            if (blog.Photo == null)
            {
                ModelState.AddModelError("Photo", "please select file");
                return View();
            }
            if (!blog.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "please select image file");
                return View();
            }
            if (blog.Photo.IsBigger2MB())
            {
                ModelState.AddModelError("Photo", "select smaller image");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "assets", "images", "faces");
            blog.Image = await blog.Photo.SaveImageAsync(folder);

            blog.Status = true;
            blog.CreatedDate = DateTime.Parse(DateTime.Now.ToShortDateString());
            await _blogWrite.AddAsync(blog);
            await _blogWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Blog dbBlog = await _blogRead.GetByIdAsync(id);
            if (dbBlog == null)
            {
                return BadRequest();
            }
            ViewBag.Writers = await _writerRead.GetAll().Include(x => x.Blogs).ToListAsync();
            ViewBag.Categories = await _categoryRead.GetAll().Include(x => x.Blogs).ToListAsync();
            return View(dbBlog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Blog blog, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Blog dbBlog = await _blogRead.GetByIdAsync(id);
            if (dbBlog == null)
            {
                return BadRequest();
            }
            ViewBag.Writers = await _writerRead.GetAll().Include(x => x.Blogs).ToListAsync();
            ViewBag.Categories = await _categoryRead.GetAll().Include(x => x.Blogs).ToListAsync();
            bool isExist = await _blogRead.IsExist(blog.Id);
            if (isExist && blog.Title!=dbBlog.Title)
            {
                ModelState.AddModelError("Name", "This blog already exists");
                return View();
            }
            if (blog.Photo != null)
            {
                if (!blog.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "please select image file");
                    return View();
                }
                if (blog.Photo.IsBigger2MB())
                {
                    ModelState.AddModelError("Photo", "select smaller image");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "images", "faces");
                Extensions.DeleteFile(folder, dbBlog.Image);
                dbBlog.Image = await blog.Photo.SaveImageAsync(folder);
            }
            else
            {
                blog.Image = dbBlog.Image;
            }
            blog.LastUpdatedDate = DateTime.UtcNow;
            blog.Status = true;
            _db.Entry(dbBlog).CurrentValues.SetValues(blog);
            await _blogWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Blog dbBlog = await _blogRead.GetWhere(x => x.Id == id).FirstOrDefaultAsync();
            if (dbBlog == null)
            {
                return BadRequest();
            }

            if (dbBlog.Status)
            {
                dbBlog.Status = false;
            }
            else
            {
                dbBlog.Status = true;
            }


            await _blogWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin ")]
        public async Task<IActionResult> Details(int id)
        {
            Blog blog = await _blogRead.GetWhere(p => p.Id == id).Include(x=>x.Category).Include(x=>x.Writer).FirstOrDefaultAsync();
            if (blog == null)
            {
                return BadRequest();
            }
            return View(blog);
        }
        public async Task<IActionResult> FilterAndSearchBlogs(string year, string category, string key)
        {
            IQueryable<Blog> blogs = _blogRead.GetAll().Include(p => p.Category).Include(x => x.Writer);
            if(!string.IsNullOrEmpty(category) && category!="all" && !string.IsNullOrEmpty(year) && year!="all")
            {
                ViewBag.SelectedYear = year;
                ViewBag.SelectedCategory = category;
                blogs = blogs.Where(x => x.CreatedDate.Year.ToString() == year && x.Category.Name == category);
            }
            else if(category=="all" && year == "all")
            {
                ViewBag.SelectedYear = year;
                ViewBag.SelectedCategory = category;
            }
            else if (!string.IsNullOrEmpty(category) && year == "all")
            {
                ViewBag.SelectedYear = year;
                ViewBag.SelectedCategory = category;
                blogs = blogs.Where(x=>x.Category.Name==category);
            }
            else if (category == "all" && !string.IsNullOrEmpty(year))
            {
                ViewBag.SelectedYear = year;
                ViewBag.SelectedCategory = category;
                blogs = blogs.Where(x=>x.CreatedDate.Year.ToString()==year);
            }
            else if (!string.IsNullOrEmpty(category) && year == null)
            {
                ViewBag.SelectedYear = year;
                ViewBag.SelectedCategory = category;
                blogs = blogs.Where(x=>x.Category.Name==category);
            }
            else if (!string.IsNullOrEmpty(year) && category == null)
            {
                ViewBag.SelectedYear = year;
                ViewBag.SelectedCategory = category;
                blogs = blogs.Where(x => x.CreatedDate.Year.ToString() == year).Include(x => x.Writer).Include(x => x.Category);
            }
            if (!string.IsNullOrEmpty(key))
            {
                ViewBag.Key = key;
                blogs = blogs.Where(x => x.Title.ToLower().Contains(key.ToLower()));
            }
            else
            {
                blogs = blogs.OrderByDescending(x => x.Id).Take(3);
            }


            return PartialView("_BlogList",await blogs.ToListAsync());
        }
        public async Task<IActionResult> Pagination(int page)
        {
            int showCount = 3;
            IQueryable<Blog> blogs = _blogRead.GetAll().OrderByDescending(x => x.Id).Skip((page - 1) * showCount).Take(showCount).Include(p => p.Category).Include(x => x.Writer);
            return PartialView("_BlogPagination",await blogs.ToListAsync());
        }
        //public async Task<IActionResult> SearchBlogs()
        //{

        //    return PartialView("_BlogSearch",blogs)
        //}
        //public IActionResult FilterByYear(string year)
        //{
        //    IQueryable<Blog> blogs = _blogRead.GetAll();

        //    if (year != "all")
        //    {
        //        int yearInt = int.Parse(year);
        //        blogs = blogs.Where(b => b.CreatedDate.Year == yearInt);
        //    }

        //    return PartialView("_BlogListPartial", blogs.ToList());
        //}
        //public IActionResult List(DateTime? selectedDate)
        //{
        //    IQueryable<Blog> blogs = _blogRead.GetAll();

        //    if (selectedDate.HasValue)
        //    {
        //        blogs = blogs.Where(b => b.CreatedDate.Year == selectedDate.Value.Year);
        //    }

        //    List<int> dates = blogs
        //        .Select(b => b.CreatedDate.Year)
        //        .Distinct()
        //        .OrderByDescending(date => date)
        //        .ToList();

        //    BlogListVM viewModel = new BlogListVM
        //    {
        //        Blogs = blogs.OrderByDescending(b => b.CreatedDate).ToList(),
        //        Dates = dates,
        //        SelectedDate = selectedDate
        //    };
        //    if(HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    {
        //        return PartialView("_BlogList", viewModel);
        //    }
        //    return View(viewModel);

        //}
        //if (categoryId!="all" && !string.IsNullOrEmpty(categoryId))
        //{
        //    int categoryInt = int.Parse(categoryId);
        //    blogs = blogs.Where(p=>p.Category.Id == categoryInt).ToList();
        ////}
        //IQueryable<Blog> blogs = _blogRead.GetAll();

        //if (year.HasValue)
        //{
        //    blogs = blogs.Where(b => b.CreatedDate.Year == year.Value.Year);
        //}

    }
}
