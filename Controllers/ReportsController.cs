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
    public class ReportsController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public ReportsController(PJInstallationDbContext context)
        {
            _context = context;
        }

        // GET: Reports/Inventory
        public async Task<IActionResult> Inventory()
        {
            var reportData = await _context.PurchaseItems
                .Include(pi => pi.Product)
                .Include(pi => pi.Purchase)
                .GroupBy(pi => new { pi.Product.Description, pi.Product.Name })
                .Select(g => new InventoryReportViewModel
                {
                    Category = g.Key.Description,
                    Brand = g.Key.Name,
                    Quantity = g.Sum(pi => pi.Stack) // Use Stack for available stock
                })
                .OrderBy(r => r.Category)
                .ToListAsync();

            return View(reportData);
        }




        public async Task<IActionResult> Financial(DateTime? from, DateTime? to)
        {
            var query = _context.CustomerPurchases
                .Include(cp => cp.CustomerPurchaseItems)
                    .ThenInclude(i => i.Product)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(cp => cp.PurchaseDate >= from.Value);

            if (to.HasValue)
                query = query.Where(cp => cp.PurchaseDate <= to.Value);

            var purchases = await query
                .Select(cp => new FinancialReportViewModel
                {
                    PurchaseOrder = cp.OrderId,
                    PurchaseDate = cp.PurchaseDate,
                    Total = cp.CustomerPurchaseItems.Sum(i => i.Price * i.Quantity)
                            + (cp.LaborFeeAmount ?? 0)
                })
                .ToListAsync();

            ViewBag.GrandTotal = purchases.Sum(p => p.Total);

            // ✅ Pass filter values back
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            return View(purchases);
        }

    }
}
