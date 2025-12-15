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
    public class CustomerPurchasesController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public CustomerPurchasesController(PJInstallationDbContext context)
        {
            _context = context;
        }

        // GET: CustomerPurchases
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 5;
            var query = _context.CustomerPurchases.AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c =>
                    (c.CustomerProject != null && c.CustomerProject.Contains(searchString)));
                ViewBag.SearchString = searchString;
            }

            int totalPurchases = await query.CountAsync();

            var purchases = await query
                .OrderByDescending(c => c.CustomerPurchaseId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPurchases / (double)pageSize);
            ViewBag.TotalPurchases = totalPurchases;
            ViewBag.PageSize = pageSize;

            return View(purchases);
        }


        // GET: CustomerPurchases/Create
        public IActionResult Create()
        {
            ViewBag.Products = _context.Products
                .Include(p => p.ProductSuppliers)
                .Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.ProductSuppliers.Select(ps => ps.Price).FirstOrDefault(),
                    AvailableStock = _context.PurchaseItems
                        .Where(pi => pi.ProductId == p.ProductId)
                        .Sum(pi => pi.Stack) // ✅ Use Stack for inventory
                })
                .ToList();

            return View();
        }

        // POST: CustomerPurchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string customerProject,
            string description,
            string status,
            string? installationLocation,
            DateTime? installationDate,
            DateTime? installationEndDate,
            decimal? laborFeeAmount,
            List<int> productIds,
            List<int> quantities,
            List<decimal> prices)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(customerProject))
                ModelState.AddModelError("CustomerProject", "Customer / Project is required.");

            if (productIds == null || productIds.Count == 0)
                ModelState.AddModelError("", "Please add at least one product.");

            if (quantities == null || quantities.Count != productIds.Count)
                ModelState.AddModelError("", "Quantity mismatch.");

            if (prices == null || prices.Count != productIds.Count)
                ModelState.AddModelError("", "Price mismatch.");

            if (status == "Installation")
            {
                if (string.IsNullOrEmpty(installationLocation))
                    ModelState.AddModelError("InstallationLocation", "Installation location is required.");
                if (!installationDate.HasValue)
                    ModelState.AddModelError("InstallationDate", "Installation date is required.");
                if (!installationEndDate.HasValue)
                    ModelState.AddModelError("InstallationEndDate", "Installation end date is required.");
                else if (installationEndDate <= installationDate)
                    ModelState.AddModelError("InstallationEndDate", "End date must be after installation date.");
            }

            if (!ModelState.IsValid)
            {
                // Repopulate products for dropdown if validation fails
                ViewBag.Products = _context.Products
                    .Include(p => p.ProductSuppliers)
                    .Select(p => new ProductDto
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.ProductSuppliers.Select(ps => ps.Price).FirstOrDefault(),
                        AvailableStock = _context.PurchaseItems
                            .Where(pi => pi.ProductId == p.ProductId)
                            .Sum(pi => pi.Stack) // ✅ Use Stack
                    })
                    .ToList();

                return View();
            }

            // Generate next OrderId
            var lastOrder = await _context.CustomerPurchases
                .OrderByDescending(p => p.CustomerPurchaseId)
                .FirstOrDefaultAsync();

            int nextNum = 1;
            if (lastOrder != null && !string.IsNullOrEmpty(lastOrder.OrderId))
            {
                string lastNumber = lastOrder.OrderId.Replace("PO-", "");
                if (int.TryParse(lastNumber, out int parsed)) nextNum = parsed + 1;
            }

            // Map InstallationStatus
            string installationStatusDisplay = status == "Installation"
                ? "With Installation"
                : "Without Installation";

            // Create CustomerPurchase entity
            var purchase = new CustomerPurchase
            {
                OrderId = "PO-" + nextNum.ToString("D3"),
                CustomerProject = customerProject,
                Status = installationStatusDisplay,
                InstallationStatus = "Pending",
                PurchaseDate = DateTime.Now,
                InstallationLocation = installationLocation,
                InstallationDate = installationDate,
                InstallationEndDate = installationEndDate,
                LaborFeeAmount = status == "Installation" ? laborFeeAmount : null,
                CustomerPurchaseItems = new List<CustomerPurchaseItem>()
            };

            // Add purchase items and deduct stock (FIFO using Stack)
            for (int i = 0; i < productIds.Count; i++)
            {
                var productId = productIds[i];
                var qty = quantities[i];
                var price = prices[i];

                // Get available stock items (FIFO)
                var stockItems = await _context.PurchaseItems
                    .Where(pi => pi.ProductId == productId && pi.Stack > 0)
                    .OrderBy(pi => pi.PurchaseItemId)
                    .ToListAsync();

                int totalAvailable = stockItems.Sum(s => s.Stack);

                if (totalAvailable < qty)
                {
                    ModelState.AddModelError("", $"Not enough stock for product ID {productId}. Requested: {qty}, Available: {totalAvailable}");

                    // Repopulate dropdown again
                    ViewBag.Products = _context.Products
                        .Include(p => p.ProductSuppliers)
                        .Select(p => new ProductDto
                        {
                            ProductId = p.ProductId,
                            Name = p.Name,
                            Description = p.Description,
                            Price = p.ProductSuppliers.Select(ps => ps.Price).FirstOrDefault(),
                            AvailableStock = _context.PurchaseItems
                                .Where(pi => pi.ProductId == p.ProductId)
                                .Sum(pi => pi.Stack) // ✅ Use Stack
                        })
                        .ToList();

                    return View();
                }

                // Deduct stock across batches (FIFO)
                int remaining = qty;
                foreach (var stock in stockItems)
                {
                    if (remaining <= 0) break;

                    if (stock.Stack >= remaining)
                    {
                        stock.Stack -= remaining;
                        remaining = 0;
                    }
                    else
                    {
                        remaining -= stock.Stack;
                        stock.Stack = 0;
                    }
                }

                // Add to CustomerPurchaseItems
                purchase.CustomerPurchaseItems.Add(new CustomerPurchaseItem
                {
                    ProductId = productId,
                    Quantity = qty,
                    Price = price
                });
            }

            // Save everything
            _context.CustomerPurchases.Add(purchase);
            await _context.SaveChangesAsync();

            // Redirect to Payment
            return RedirectToAction("Payment", new { id = purchase.CustomerPurchaseId });
        }





        // GET: CustomerPurchases/Payment/5
        public async Task<IActionResult> Payment(int id)
        {
            var purchase = await _context.CustomerPurchases
                .Include(p => p.CustomerPurchaseItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.CustomerPurchaseId == id);

            if (purchase == null)
            {
                return NotFound();
            }

            // Compute product total
            decimal productTotal = purchase.CustomerPurchaseItems.Sum(i => i.Price * i.Quantity);

            // Use manual labor fee (only for Installation)
            decimal laborFee = (purchase.Status == "Installation" && purchase.LaborFeeAmount.HasValue)
                ? purchase.LaborFeeAmount.Value
                : 0;

            // Compute grand total
            purchase.GrandTotal = productTotal + laborFee;

            // Save updates
            _context.Update(purchase);
            await _context.SaveChangesAsync();

            return View(purchase); // loads Payment
        }

        // POST: CustomerPurchases/ConfirmPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int CustomerPurchaseId, string PaymentMethod, decimal AmountReceived)
        {
            var purchase = await _context.CustomerPurchases
                .FirstOrDefaultAsync(p => p.CustomerPurchaseId == CustomerPurchaseId);

            if (purchase == null)
            {
                return NotFound();
            }

            // Compute grand total again (for validation)
            decimal productTotal = _context.CustomerPurchaseItems
                .Where(i => i.CustomerPurchaseId == CustomerPurchaseId)
                .Sum(i => i.Price * i.Quantity);

            decimal laborFee = (purchase.Status == "Installation" && purchase.LaborFeeAmount.HasValue)
                ? purchase.LaborFeeAmount.Value
                : 0;

            decimal grandTotal = productTotal + laborFee;

            // Store payment details
            purchase.PaymentDate = DateTime.Now;
            purchase.AmountReceived = AmountReceived;
            purchase.GrandTotal = grandTotal;

            // Calculate change
            purchase.ChangeAmount = AmountReceived - grandTotal;

            // Mark as Completed right away (remove Pending)
            purchase.PaymentStatus = "Completed";

            _context.Update(purchase);
            await _context.SaveChangesAsync();

            // ✅ Go back to Index after successful payment
            return RedirectToAction("Index");
        }


        // GET: CustomerPurchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var purchase = await _context.CustomerPurchases
                .Include(p => p.CustomerPurchaseItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(m => m.CustomerPurchaseId == id);

            if (purchase == null) return NotFound();

            // ViewBags for small info display
            ViewBag.CustomerProject = purchase.CustomerProject;
            ViewBag.PurchaseDate = purchase.PurchaseDate.ToString("yyyy-MM-dd");
            ViewBag.InstallationLocation = purchase.InstallationLocation ?? "-";
            ViewBag.InstallationDate = purchase.InstallationDate?.ToString("yyyy-MM-dd") ?? "-";
            ViewBag.InstallationEndDate = purchase.InstallationEndDate?.ToString("yyyy-MM-dd") ?? "-";
            ViewBag.PurchaseLocation = purchase.InstallationLocation ?? "-";

            return View(purchase);
        }

        // GET: CustomerPurchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerPurchase = await _context.CustomerPurchases
                .FirstOrDefaultAsync(m => m.CustomerPurchaseId == id);
            if (customerPurchase == null)
            {
                return NotFound();
            }

            return View(customerPurchase);
        }

        // POST: CustomerPurchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerPurchase = await _context.CustomerPurchases.FindAsync(id);
            if (customerPurchase != null)
            {
                _context.CustomerPurchases.Remove(customerPurchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
                private bool CustomerPurchaseExists(int id)
        {
            return _context.CustomerPurchases.Any(e => e.CustomerPurchaseId == id);
        }
    }
}
