using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Identity;
using Application.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using static Main.Helpers.Helper;

namespace Main.Controllers
{
    public class ExpenditureController : Controller
    {
        private readonly IExpenditureReadRepository _expenditureRead;
        private readonly IExpenditureWriteRepository _expenditureWrite;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;
        public ExpenditureController(IExpenditureReadRepository expenditureRead , IExpenditureWriteRepository expenditureWrite , UserManager<AppUser> userManager,AppDbContext db)
        {
            _expenditureRead = expenditureRead;
            _expenditureWrite = expenditureWrite;
            _userManager = userManager;
            _db = db;
        }
        #region Expenditure's Index
        [Authorize(Roles = ("Admin"))]
        public IActionResult Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal)_expenditureRead.GetAll().Count() / 3);
            List<Expenditure> expenditures = _expenditureRead.GetAll().Skip((page - 1) * 3).Take(3).Include(x => x.AppUser).ToList();
            return View(expenditures);
        }

        #endregion

        #region Expenditure's Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Expenditure expenditure)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Cash cash = await _db.Cashes.FirstOrDefaultAsync();
            cash.LastModifiedBy = user.Name;

            if (expenditure.Money > cash.Balance)
            {
                ModelState.AddModelError("Money", "Balansda kifayet qeder Pul yoxdur.");
                return View();
            }
            else
            {
                cash.Balance -= expenditure.Money;
            }

            cash.LastMotifiedMoney = expenditure.Money - expenditure.Money - expenditure.Money;
            cash.LastModified = expenditure.Description;
            cash.LastModifiedTime = expenditure.ExpenditureTime = DateTime.UtcNow.AddHours(4);
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            expenditure.AppUserId = appUser.Id;
            await _db.Expenditures.AddAsync(expenditure);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion


    }
}
