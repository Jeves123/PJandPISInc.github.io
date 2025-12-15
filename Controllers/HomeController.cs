using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using PJ_P_Installation_Management_System.Data;
using PJ_P_Installation_Management_System.Models;
using PJ_P_Installation_Management_System.ViewModel;

namespace PJ_P_Installation_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public HomeController(PJInstallationDbContext context) // ? use your actual DbContext
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalSuppliers = await _context.Suppliers.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalPurchases = await _context.Purchases.CountAsync(),
                TotalCustomerPurchases = await _context.CustomerPurchases.CountAsync(),
                TotalStaff = await _context.Staffs.CountAsync(),
                TotalSchedules = await _context.Schedules.CountAsync(),
                TotalInstallations = await _context.CustomerPurchases
                                        .CountAsync(c => c.Status == "With Installation")
            };

            return View(dashboard);
        }
    }
}
