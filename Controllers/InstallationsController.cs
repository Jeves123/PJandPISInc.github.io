using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PJ_P_Installation_Management_System.Data;
using PJ_P_Installation_Management_System.Models;
using PJ_P_Installation_Management_System.ViewModel;


namespace PJ_P_Installation_Management_System.Controllers
{
    public class InstallationsController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public InstallationsController(PJInstallationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 5;

            // Only fetch purchases that are WITH INSTALLATION
            var query = _context.CustomerPurchases
                .Where(c => c.Status == "With Installation")   // <- key change
                .Select(c => new InstallationViewModel
                {
                    CustomerPurchaseId = c.CustomerPurchaseId,
                    OrderId = c.OrderId,
                    CustomerProject = c.CustomerProject,
                    InstallationStatus = c.InstallationStatus ?? "Pending", // default Pending
                    Status = c.Status
                });

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.CustomerProject.Contains(searchString)
                                      || c.OrderId.Contains(searchString));
            }

            int total = await query.CountAsync();

            var installations = await query
                .OrderBy(c => c.OrderId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.SearchString = searchString;

            return View(installations);
        }


        // GET: Installations/EditStatus/5
        public async Task<IActionResult> EditStatus(int id)
        {
            var purchase = await _context.CustomerPurchases
                .FirstOrDefaultAsync(p => p.CustomerPurchaseId == id);

            if (purchase == null) return NotFound();

            var model = new InstallationViewModel
            {
                CustomerPurchaseId = purchase.CustomerPurchaseId,
                OrderId = purchase.OrderId,
                CustomerProject = purchase.CustomerProject,
                InstallationStatus = purchase.InstallationStatus // <-- updated
            };

            // Only allow Pending, Ongoing, Completed
            ViewBag.StatusOptions = new SelectList(
                new List<string> { "Pending", "Ongoing", "Completed" },
                purchase.InstallationStatus
            );

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(int id, string installationStatus)
        {
            var purchase = await _context.CustomerPurchases
                .FirstOrDefaultAsync(p => p.CustomerPurchaseId == id);

            if (purchase == null) return NotFound();

            // Only allow Pending, Ongoing, Completed
            if (installationStatus == "Pending" || installationStatus == "Ongoing" || installationStatus == "Completed")
            {
                purchase.InstallationStatus = installationStatus; // <-- updated
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Installation status updated!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
