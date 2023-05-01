using Application.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Main.Areas
{
    [Area("Comment")]
    public class CommentController : Controller
    {
        private readonly IBlogReadRepository _blogRead;
        private readonly ICommentWriteRepository _commentWrite;
        private readonly ICommentReadRepository _commentRead;
        public CommentController(IBlogReadRepository blogRead, ICommentWriteRepository commentWrite ,ICommentReadRepository commentRead)
        {
            _commentWrite = commentWrite;
            _blogRead = blogRead;
            _commentRead = commentRead;
        }
        [Authorize(Roles = "Admin , Customer")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Blogs = await _blogRead.GetAll().ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(Comment comment)
        {
            bool check = true;
            if (comment.Title == null)
            {
                ModelState.AddModelError("Title", "Title cannot be empty");
            }
            if (comment.Content == null)
            {
                ModelState.AddModelError("Content", "Content cannot be null");
            }
            comment.Username = HttpContext.Session.GetString("Username");
            comment.Status = true;
            comment.CreatedDate = DateTime.UtcNow;
            check = await _commentWrite.AddAsync(comment);
            await _commentWrite.SaveChangesAsync();
            if(check)
            {
                return RedirectToAction("Successfull");
            }
            return View(comment);
            
        }
        public IActionResult Successfull()
        {
            return View();
        }

    }
}
