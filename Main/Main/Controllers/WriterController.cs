using Application.Repositories;
using Domain.Entities;
using Main.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace Main.Controllers
{
    public class WriterController : Controller
    {
        private readonly IWriterReadRepository _writerRead;
        private readonly IWriterWriteRepository _writerWrite;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _db;
        public WriterController(IWriterReadRepository readRepository , IWriterWriteRepository writeRepository , IWebHostEnvironment env, AppDbContext db)
        {
            _writerRead = readRepository; 
            _writerWrite = writeRepository;
            _env = env;
            _db = db;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            int showCount = 3;
            ViewBag.PageCount = Math.Ceiling((Decimal)await _writerRead.CountAsync() / showCount);
            IQueryable<Writer> writers = _writerRead.GetAll().OrderByDescending(x => x.Id).Take(showCount);
            return View(await writers.ToListAsync());
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Writer dbWriter = await _writerRead.GetWhere(x => x.Id == id).FirstOrDefaultAsync();
            if (dbWriter == null)
            {
                return BadRequest();
            }

            if (dbWriter.Status)
            {
                dbWriter.Status = false;
            }
            else
            {
                dbWriter.Status = true;
            }


            await _writerWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            Writer writer = await _writerRead.GetByIdAsync(id);
            if (writer == null)
            {
                return BadRequest();
            }
            return View(writer);
        }
        public async Task<IActionResult> Pagination(int page = 1)
        {
            int showCount = 3;
            IQueryable<Writer> writers = _writerRead.GetAll().OrderByDescending(x => x.Id).Skip((page - 1) * showCount).Take(showCount);
            return PartialView("_WriterPagination", await writers.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Writer writer)
        {
            ViewBag.BlogsWithCategories = await _writerRead.GetAll().ToListAsync();
            bool isExist = await _writerRead.IsExist(writer.Id);
            if (isExist)
            {
                ModelState.AddModelError("Name", "This blog already exists");
                return View();
            }
            if (writer.Photo == null)
            {
                ModelState.AddModelError("Photo", "please select file");
                return View();
            }
            if (!writer.Photo.IsImage())
            {
                ModelState.AddModelError("Photo", "please select image file");
                return View();
            }
            if (writer.Photo.IsBigger2MB())
            {
                ModelState.AddModelError("Photo", "select smaller image");
                return View();
            }
            string folder = Path.Combine(_env.WebRootPath, "assets", "images", "faces");
            writer.Image = await writer.Photo.SaveImageAsync(folder);

            writer.Status = true;
            await _writerWrite.AddAsync(writer);
            await _writerWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Writer dbWriter = await _writerRead.GetByIdAsync(id);
            if (dbWriter == null)
            {
                return BadRequest();
            }
            return View(dbWriter);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Writer writer , int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Writer dbWriter = await _writerRead.GetByIdAsync(id);
            if (dbWriter == null)
            {
                return BadRequest();
            }
            //bool isExist = await _writerRead.IsExist(writer.Id);
            //if (isExist && dbWriter.Name!=writer.Name)
            //{
            //    ModelState.AddModelError("Name", "This blog already exists");
            //    return View();
            //}
            if (writer.Photo != null)
            {
                if (!writer.Photo.IsImage())
                {
                    ModelState.AddModelError("Photo", "please select image file");
                    return View();
                }
                if (writer.Photo.IsBigger2MB())
                {
                    ModelState.AddModelError("Photo", "select smaller image");
                    return View();
                }
                string folder = Path.Combine(_env.WebRootPath, "assets", "images", "faces");
                Extensions.DeleteFile(folder, dbWriter.Image);
                dbWriter.Image = await writer.Photo.SaveImageAsync(folder);
            }
            else
            {
                writer.Image = dbWriter.Image;
            }
            writer.Status = true;
            _db.Entry(dbWriter).CurrentValues.SetValues(writer);
            await _writerWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
