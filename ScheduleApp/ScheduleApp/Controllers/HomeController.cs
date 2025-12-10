using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Data;
using ScheduleApp.Models;

namespace ScheduleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly Context _context;

        public HomeController(Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }

        // Метод для получения деталей страны
        public JsonResult GetCountryDetails(int id)
        {
            var country = _context.Roles.FirstOrDefault(c => c.Id == id);
            return Json(country);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
