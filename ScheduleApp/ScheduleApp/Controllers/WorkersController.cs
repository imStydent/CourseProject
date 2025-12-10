using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ScheduleApp.Data;
using ScheduleApp.Models;

namespace ScheduleApp.Controllers
{
    [Authorize]
    public class WorkersController : Controller
    {
        private readonly Context _context;

        public WorkersController(Context context)
        {
            _context = context;
        }

        // GET: Workers
        public async Task<IActionResult> Index(string sortOrder, string searchString, string filter)
        {
            var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleId");
            if (roleIdClaim == null)
                return View("~/Views/Account/Login.cshtml");

            int roleId = int.Parse(roleIdClaim.Value);

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            var context = await _context.Workers.Include(w => w.Role).ToListAsync();
            if(filter != null)
            {
                context = context.Where(w => w.Role.Name == filter).ToList();
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                context = context.Where(x => x.Surname.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            switch (sortOrder)
            {
                case "name_desc":
                    context = context.OrderByDescending(s => s.Name).ToList();
                    break;
                default:
                    context = context.OrderBy(s => s.Name).ToList();
                    break;
            }

            TempData["roleId"] = roleId;
            return View(context);
        }

        // GET: Workers/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Workers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Surname,Name,Patronymic,PhoneNumber,RoleId,Password")] Worker worker)
        {
            _context.Add(worker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Workers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Workers.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", worker.RoleId);
            return View(worker);
        }

        // POST: Workers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Surname,Name,Patronymic,PhoneNumber,RoleId,Password")] Worker worker)
        {
            if (id != worker.Id)
            {
                return NotFound();
            }
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
        }

        // POST: Workers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker != null)
            {
                _context.Workers.Remove(worker);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkerExists(int id)
        {
            return _context.Workers.Any(e => e.Id == id);
        }
    }
}
