using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence.Context;
using System.Collections.Generic;
using System;
using Application.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Main.Controllers
{
    public class IncomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IIncomeReadRepository _incomeRead;
        private readonly IIncomeWriteRepository _incomeWrite;
        private readonly UserManager<AppUser> _userManager;
        public IncomeController(AppDbContext db, UserManager<AppUser> userManager, IIncomeWriteRepository incomeWrite , IIncomeReadRepository incomeRead )
        {
            _incomeRead = incomeRead;
            _incomeWrite = incomeWrite;
            _userManager = userManager;
            _db = db;
        }


        #region Income's Index
        [Authorize(Roles =("Admin"))]
        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageCount = Math.Ceiling((decimal) _incomeRead.GetAll().Count() / 3);
            List<Income> incomes =await _incomeRead.GetAll().Skip((page - 1) * 3).Take(3).Include(x => x.AppUser).ToListAsync();
            return View(incomes);
        }

        #endregion

        #region Income's Create

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Income income)
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            Cash cash = await _db.Cashes.FirstOrDefaultAsync();

            cash.LastModifiedBy = user.Name;
            cash.Balance += income.Money;
            cash.LastMotifiedMoney = income.Money;
            cash.LastModified = income.Description;
            cash.LastModifiedTime = income.IncomeTime = DateTime.UtcNow.AddHours(4);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            income.AppUserId = appUser.Id;
            await _db.Incomes.AddAsync(income);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        #endregion

    }
}
