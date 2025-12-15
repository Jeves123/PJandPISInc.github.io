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
    public class StaffsController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public StaffsController(PJInstallationDbContext context)
        {
            _context = context;
        }

        // GET: Staffs
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 5;

            var staffQuery = _context.Staffs.AsQueryable();

            // 🔍 Apply search
            if (!string.IsNullOrEmpty(searchString))
            {
                staffQuery = staffQuery.Where(s =>
                    s.FullName.Contains(searchString) ||
                    s.Position.Contains(searchString) ||
                    s.ContactNumber.Contains(searchString) ||
                    s.Email.Contains(searchString));
            }

            var totalStaff = await staffQuery.CountAsync();

            // 📄 Apply pagination
            var staffList = await staffQuery
                .OrderBy(s => s.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(totalStaff / (double)pageSize));
            ViewBag.TotalStaff = totalStaff;
            ViewBag.SearchString = searchString;

            return View(staffList);
        }


        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .FirstOrDefaultAsync(m => m.StaffId == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            ViewBag.Positions = new List<string>
    {
        // Cebu Main Office
        "Finance Manager/Purchaser",
        "Sales & Marketing Manager",
        "HR Personnel/Operations",
        "Accounting Manager",
        "Sales & Marketing Officer",
        "Security Staff",
        "Admin/Finance Personnel",
        "Property Custodian",
        "Liason Officer",
        "Accounting Personnel",

        // Sarangani Office
        "Sales & Marketing Officer",

        // Engineering
        "Engineering Supervisor",
        "Fire Protection & HVAC Design & Estimate",
        "CAD Operator Design & Estimate",

        // Operations
        "Senior Safety Officer/Quality Safety Officer",
        "Project-in-Charge",
        "Project-in-Charge/Safety Officer",

        // Old options
        "Welder",
        "Helper",
        "Pipe Fitter",
        "Electrician",
        "Mechanical"
    };

            return View();
        }

        // POST: Staffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Position,ContactNumber,Email,Gender,Location")] Staff staff)
        {
            if (ModelState.IsValid)
            {
                // Do NOT assign StaffId manually, let DB handle it
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Positions = new List<string>
            {
                "Finance Manager/Purchaser", "Sales & Marketing Manager", "HR Personnel/Operations",
                "Accounting Manager", "Sales & Marketing Officer", "Security Staff", "Admin/Finance Personnel",
                "Property Custodian", "Liason Officer", "Accounting Personnel", "Sales & Marketing Officer",
                "Engineering Supervisor", "Fire Protection & HVAC Design & Estimate", "CAD Operator Design & Estimate",
                "Senior Safety Officer/Quality Safety Officer", "Project-in-Charge", "Project-in-Charge/Safety Officer",
                "Welder","Helper", "Pipe Fitter", "Electrician", "Mechanical"
            };

            return View(staff);
        }




        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            ViewBag.Positions = new List<string>
            {
                // Cebu Main Office
                "Finance Manager/Purchaser",
                "Sales & Marketing Manager",
                "HR Personnel/Operations",
                "Accounting Manager",
                "Sales & Marketing Officer",
                "Security Staff",
                "Admin/Finance Personnel",
                "Property Custodian",
                "Liason Officer",
                "Accounting Personnel",

                // Sarangani Office
                "Sales & Marketing Officer",

                // Engineering
                "Engineering Supervisor",
                "Fire Protection & HVAC Design & Estimate",
                "CAD Operator Design & Estimate",

                // Operations
                "Senior Safety Officer/Quality Safety Officer",
                "Project-in-Charge",
                "Project-in-Charge/Safety Officer",

                // Old options you had
                "Welder",
                "Helper",
                "Pipe Fitter",
                "Electrician",
                "Mechanical"
            };

            return View(staff);
        }


        // POST: Staffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Staffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StaffId,FullName,Position,ContactNumber,Email,Gender,Location")] Staff staff)
        {
            if (id != staff.StaffId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.StaffId))
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
            return View(staff);
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .FirstOrDefaultAsync(m => m.StaffId == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staffs.FindAsync(id);
            if (staff != null)
            {
                _context.Staffs.Remove(staff);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(int id)
        {
            return _context.Staffs.Any(e => e.StaffId == id);
        }
    }
}
