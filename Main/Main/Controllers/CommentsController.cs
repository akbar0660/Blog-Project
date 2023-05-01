using Application.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentReadRepository _commentRead;
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICommentWriteRepository _commentWrite;
        public CommentsController(ICommentReadRepository comment , AppDbContext db, UserManager<AppUser> userManager,ICommentWriteRepository commentWrite)
        {
            _db = db;
            _commentRead = comment;
            _userManager = userManager;
            _commentWrite = commentWrite;
        }
        public async Task<IActionResult> Index()
        {
            int showCount = 3;
            IQueryable<Comment> comments = _commentRead.GetAll().OrderByDescending(x => x.Id).Take(showCount).Include(x => x.Blog);
            ViewBag.PageCount =Math.Ceiling((Decimal)await _commentRead.GetAll().CountAsync() / showCount);
            return View( await comments.ToListAsync());
        }
        public async Task<IActionResult> Pagination(int page)
        {
            int showCount = 3;
            IQueryable<Comment> comments = _commentRead.GetAll().OrderByDescending(x => x.Id).Skip((page-1)*showCount).Take(showCount);
            return PartialView("_CommentPagination",await comments.ToListAsync());
        }
        public async Task<IActionResult> Details(int id)
        {
            Comment comment =await _commentRead.GetWhere(x=>x.Id==id).Include(x => x.Blog).FirstOrDefaultAsync();
            if (comment == null)
            {
                return BadRequest();
            }
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Username = user.UserName;
            return View(comment);
        }
        public async Task<IActionResult> Activity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Comment dbComment = await _commentRead.GetByIdAsync(id);
            if (dbComment == null)
            {
                return BadRequest();
            }

            if (dbComment.Status)
            {
                dbComment.Status = false;
            }
            else
            {
                dbComment.Status = true;
            }


            await _commentWrite.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
