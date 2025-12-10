using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleApp.Data;
using ScheduleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ScheduleApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly Context _context;

        public AccountController(Context context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            var user = await _context.Workers.FirstOrDefaultAsync(emp => emp.PhoneNumber == model.Login);
            if (user == null)
                return View();
            if (user.Password != model.Password)
                return View();

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Login),
                    new Claim("RoleId", user.RoleId.ToString(), ClaimValueTypes.Integer32)
                };
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false // Не допускайте сохранение Cookie
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
                if (user.RoleId == 3)
                    return Redirect(returnUrl ?? "/Workers/Index");
                return Redirect(returnUrl ?? "/Schedule/Index");
            }
            ModelState.AddModelError(string.Empty, "Неверные учетные данные.");


            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                TempData["Message"] = "Пожалуйста, авторизуйтесь.";
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
