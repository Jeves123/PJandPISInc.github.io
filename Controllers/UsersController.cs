using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PJ_P_Installation_Management_System.Data;
using PJ_P_Installation_Management_System.Models;

namespace PJ_P_Installation_Management_System.Controllers
{
    public class UsersController : Controller
    {
        private readonly PJInstallationDbContext _context;

        private List<string> GetRoles()
        {
            return new List<string> { "Admin", "President", "Staff", "Purchase" };
        }

        public UsersController(PJInstallationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 5;

            var usersQuery = _context.Users.AsQueryable();

            // 🔍 Apply search
            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u =>
                    u.Username.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.Role.Contains(searchString));
            }

            var totalUsers = await usersQuery.CountAsync();

            // 📄 Apply pagination
            var users = await usersQuery
                .OrderBy(u => u.Username)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(totalUsers / (double)pageSize));
            ViewBag.TotalUsers = totalUsers;
            ViewBag.SearchString = searchString;

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.Roles = GetRoles();
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Email,Role,IsActive,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
                return NotFound();

            return View(user);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.Roles = GetRoles();
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,Role,IsActive,Password")] User user)
        {
            if (id != user.UserId)
                return NotFound();

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    existingUser.Username = user.Username;
                    existingUser.Email = user.Email;
                    existingUser.Role = user.Role;
                    existingUser.IsActive = user.IsActive;

                    // Keep existing password if the new one is empty
                    if (!string.IsNullOrWhiteSpace(user.Password))
                        existingUser.Password = user.Password;

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
                _context.Users.Remove(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
