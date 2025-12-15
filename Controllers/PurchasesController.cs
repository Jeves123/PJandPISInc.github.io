using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PJ_P_Installation_Management_System.Data;
using PJ_P_Installation_Management_System.Models;
using PJ_P_Installation_Management_System.Service;
using PJ_P_Installation_Management_System.Services;

namespace PJ_P_Installation_Management_System.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly PJInstallationDbContext _context;
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(PJInstallationDbContext context, IPurchaseService purchaseService)
        {
            _context = context;
            _purchaseService = purchaseService;
        }

        // GET: Purchases
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 5;

            var purchasesQuery = _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                purchasesQuery = purchasesQuery.Where(p =>
                    p.OrderId.Contains(searchString) ||
                    (p.Supplier != null && p.Supplier.CompanyName.Contains(searchString))
                );
            }

            var totalPurchases = await purchasesQuery.CountAsync();

            var purchases = await purchasesQuery
                .OrderByDescending(p => p.PurchaseDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalPurchases / (double)pageSize);
            ViewBag.TotalPurchases = totalPurchases;
            ViewBag.SearchString = searchString;

            return View(purchases);
        }

        // GET: Purchases/Create
        public IActionResult Create()
        {
            var suppliersWithProducts = _context.Suppliers
                .Where(s => s.ProductSuppliers.Any())
                .ToList();

            ViewBag.SupplierId = new SelectList(suppliersWithProducts, "SupplierId", "CompanyName");
            ViewBag.Products = new List<SelectListItem>();

            return View();
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int supplierId,
            List<int> productIds,
            List<int> quantities)
        {
            try
            {
                if (supplierId <= 0)
                    ModelState.AddModelError("SupplierId", "Please select a supplier.");

                if (productIds == null || !productIds.Any() || quantities == null || quantities.Count != productIds.Count)
                    ModelState.AddModelError("", "Please add at least one product with a valid quantity.");

                if (ModelState.IsValid)
                {
                    var lastOrder = await _context.Purchases
                        .OrderByDescending(p => p.PurchaseId)
                        .FirstOrDefaultAsync();

                    int nextOrderNumber = 1;
                    if (lastOrder != null && int.TryParse(lastOrder.OrderId, out int lastNumber))
                        nextOrderNumber = lastNumber + 1;

                    string orderId = nextOrderNumber.ToString("D6");

                    var purchase = new Purchase
                    {
                        SupplierId = supplierId,
                        OrderId = orderId,
                        PurchaseDate = DateTime.Now,
                        OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
                        DeliveryTrackingNumber = $"TRK-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                        PurchaseOrderNo = $"P.O {DateTime.Now.Year}-{(_context.Purchases.Count() + 1):D3}",

                        // ✅ Stack is tied to DeliveredQuantity
                        PurchaseItems = productIds.Select((pid, index) => new PurchaseItem
                        {
                            ProductId = pid,
                            Quantity = quantities[index] < 1 ? 1 : quantities[index],
                            DeliveredQuantity = 0,
                            Stack = 0 // Initialize to 0; will increase as items are delivered
                        }).ToList()
                    };

                    _context.Purchases.Add(purchase);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Purchase created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating purchase: {ex}");
                TempData["ErrorMessage"] = "Failed to create purchase. Please try again.";
            }

            var suppliersWithProducts = _context.Suppliers
                .Where(s => s.ProductSuppliers.Any())
                .ToList();

            ViewBag.SupplierId = new SelectList(suppliersWithProducts, "SupplierId", "CompanyName", supplierId);

            if (supplierId > 0)
            {
                ViewBag.Products = _context.Products
                    .Where(p => p.ProductSuppliers.Any(ps => ps.SupplierId == supplierId))
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProductId.ToString(),
                        Text = p.Name
                    })
                    .ToList();
            }
            else
            {
                ViewBag.Products = new List<SelectListItem>();
            }

            return View(new Purchase { SupplierId = supplierId });
        }

        // AJAX: Get products by supplier
        [HttpGet]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            var products = await _context.ProductSuppliers
                .Where(ps => ps.SupplierId == supplierId)
                .Include(ps => ps.Product)
                .Select(ps => new
                {
                    ps.ProductId,
                    ps.Product.Name,
                    ps.Price,
                    ps.Product.Description
                })
                .ToListAsync();

            return Json(products);
        }

        // POST: Purchases/UpdateDeliveries
        [HttpPost]
        public async Task<IActionResult> UpdateDeliveries(int purchaseId, int[] purchaseItemIds, int[] deliveredQuantities)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);

            if (purchase == null) return NotFound();

            if (purchase.IsCompleted)
            {
                TempData["Message"] = "This purchase is already completed and cannot be edited.";
                return RedirectToAction("Details", new { id = purchaseId });
            }

            for (int i = 0; i < purchaseItemIds.Length; i++)
            {
                var item = purchase.PurchaseItems.FirstOrDefault(pi => pi.PurchaseItemId == purchaseItemIds[i]);
                if (item != null)
                {
                    int newDeliveredQty = deliveredQuantities[i];

                    // Only increase Stack as DeliveredQuantity increases
                    int stockDiff = newDeliveredQty - item.DeliveredQuantity;

                    item.DeliveredQuantity = newDeliveredQty;

                    // Update Stack based on DeliveredQuantity
                    item.Stack += stockDiff;

                    if (item.Stack < 0) item.Stack = 0;
                }
            }

            if (purchase.PurchaseItems.All(pi => pi.DeliveredQuantity >= pi.Quantity))
                purchase.IsCompleted = true;

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = purchaseId });
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var purchase = await _context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null) return NotFound();

            return View(purchase);
        }   

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases

                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.PurchaseId == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase != null)
            {
                // Remove child PurchaseItems first
                if (purchase.PurchaseItems != null && purchase.PurchaseItems.Any())
                {
                    _context.PurchaseItems.RemoveRange(purchase.PurchaseItems);
                }

                _context.Purchases.Remove(purchase);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.PurchaseId == id);
        }

    }
}