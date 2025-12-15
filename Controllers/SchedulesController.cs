using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PJ_P_Installation_Management_System.Data;
using PJ_P_Installation_Management_System.Models;


namespace PJ_P_Installation_Management_System.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public SchedulesController(PJInstallationDbContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 5;

            var query = _context.Schedules
                .Include(s => s.CustomerPurchase)
                .Include(s => s.StaffAssignments)
                    .ThenInclude(sa => sa.Staff)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s =>
                    (s.TaskDescription != null && s.TaskDescription.Contains(searchString)) ||
                    (s.Location != null && s.Location.Contains(searchString)) ||
                    (s.StaffAssignments.Any(sa => sa.Staff.FullName.Contains(searchString))) ||
                    (s.CustomerPurchase.CustomerProject != null && s.CustomerPurchase.CustomerProject.Contains(searchString))
                );
            }

            int totalSchedules = await query.CountAsync();

            var schedules = await query
                .OrderByDescending(s => s.ScheduleId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalSchedules / (double)pageSize);
            ViewBag.TotalSchedules = totalSchedules;

            return View(schedules);
        }

        // Allowed staff positions (restrict which staff can be assigned)
        private static readonly List<string> AllowedPositions = new List<string>
        {
            "Engineering Supervisor",
            "Senior Safety Officer/Quality Safety Officer",
            "Project-in-Charge",
            "Project-in-Charge/Safety Officer",
            "Welder",
            "Pipe Fitter",
            "Electrician",
            "Mechanical",
            "Helper"
        };

        // GET: Schedules/AssignStaff/{purchaseId}
        public async Task<IActionResult> AssignStaff(int purchaseId)
        {
            var purchase = await _context.CustomerPurchases
                .FirstOrDefaultAsync(p => p.CustomerPurchaseId == purchaseId);

            if (purchase == null)
                return NotFound(); 

            // Only allow assignment if purchase has installation
            if (purchase.Status != "With Installation")
            {
                TempData["ErrorMessage"] = "This purchase does not require installation.";
                return RedirectToAction("Index", "CustomerPurchases");
            }

            // Pass info to the view
            ViewBag.PurchaseId = purchase.CustomerPurchaseId;
            ViewBag.CustomerProject = purchase.CustomerProject;
            ViewBag.PurchaseDate = purchase.InstallationDate?.ToString("yyyy-MM-dd") ?? "-";
            ViewBag.InstallationEndDate = purchase.InstallationEndDate?.ToString("yyyy-MM-dd") ?? "-";
            ViewBag.PurchaseLocation = purchase.InstallationLocation ?? "-";

            // Allowed positions only
            ViewBag.Positions = await _context.Staffs
                .Where(s => AllowedPositions.Contains(s.Position))
                .Select(s => s.Position)
                .Distinct()
                .ToListAsync();

            return View(); // loads AssignStaff.cshtml
        }

        // POST: Schedules/AssignStaff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStaff(int purchaseId, List<int> staffIds)
        {
            if (staffIds == null || !staffIds.Any())
            {
                ModelState.AddModelError("", "At least one staff must be selected.");
            }

            var purchase = await _context.CustomerPurchases
                .FirstOrDefaultAsync(p => p.CustomerPurchaseId == purchaseId);
            if (purchase == null) return NotFound();

            // Validate ModelState again
            if (!ModelState.IsValid)
            {
                ViewBag.Positions = await _context.Staffs
                    .Where(s => AllowedPositions.Contains(s.Position))
                    .Select(s => s.Position)
                    .Distinct()
                    .ToListAsync();

                return View(new Schedule());
            }

            // Double-check staff belong to allowed positions (security layer)
            var validStaffIds = await _context.Staffs
                .Where(s => AllowedPositions.Contains(s.Position) && staffIds.Contains(s.StaffId))
                .Select(s => s.StaffId)
                .ToListAsync();

            if (!validStaffIds.Any())
            {
                ModelState.AddModelError("", "Selected staff are not valid for assignment.");
                ViewBag.Positions = await _context.Staffs
                    .Where(s => AllowedPositions.Contains(s.Position))
                    .Select(s => s.Position)
                    .Distinct()
                    .ToListAsync();

                return View(new Schedule());
            }

            var schedule = new Schedule
            {
                ScheduledDate = purchase.InstallationDate ?? DateTime.Now,
                EndDate = purchase.InstallationEndDate,
                TaskDescription = $"Installation for {purchase.CustomerProject}",
                Location = purchase.InstallationLocation,
                CustomerPurchaseId = purchase.CustomerPurchaseId,
                StaffAssignments = validStaffIds.Select(staffId => new ScheduleStaff
                {
                    StaffId = staffId
                }).ToList()
            };

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Staff successfully assigned!";
            return RedirectToAction("Index");
        }

        // AJAX: Get staff list by position
        public async Task<JsonResult> GetStaffByPosition(string position)
        {
            var staff = await _context.Staffs
                .Where(s => AllowedPositions.Contains(s.Position) && s.Position == position)
                .Select(s => new { staffId = s.StaffId, fullName = s.FullName })
                .ToListAsync();

            return Json(staff);
        }

        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var schedule = await _context.Schedules
                .Include(s => s.StaffAssignments)
                    .ThenInclude(sa => sa.Staff)
                .Include(s => s.CustomerPurchase)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);

            if (schedule == null) return NotFound();

            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var schedule = await _context.Schedules
                .Include(s => s.CustomerPurchase)
                .Include(s => s.StaffAssignments)
                    .ThenInclude(sa => sa.Staff)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);

            if (schedule == null) return NotFound();

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.Schedules
                .Include(s => s.StaffAssignments)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (schedule == null)
            {
                return NotFound();
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Schedule deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleId == id);
        }
    }
}
