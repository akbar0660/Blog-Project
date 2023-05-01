using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System.Threading.Tasks;
using static Main.Helpers.Helper;

namespace Main.Controllers
{
    public class CashController : Controller
    {
        private readonly AppDbContext _db;
        public CashController(AppDbContext db)
        {
            _db = db;
        }

        #region Cash's Index
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Index()
        {
            Cash cashes = await _db.Cashes.FirstOrDefaultAsync();
            return View(cashes);
        }

        #endregion

    }
}
