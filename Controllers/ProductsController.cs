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
    public class ProductsController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public ProductsController(PJInstallationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 5;

            var productsQuery = _context.Products
                .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Description.Contains(searchString));
            }

            var totalProducts = await productsQuery.CountAsync();

            var products = await productsQuery
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewBag.TotalProducts = totalProducts;
            ViewBag.SearchString = searchString;

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.DescriptionList = new SelectList(new List<string>
            {
                "Sprinkler Heads Material",
                "Black Iron Pipe Material",
                "Welded Fitting Material",
                "Standard Fitting Material",
                "Reducer Fitting Material",
                "Grooved Coupling Material",
                "Polyvinyl Chloride Pipe (PVC) pipe",
                "Electrical Material",
                "Bolt Nut & Washer",
                "Valves Material"
            });

            // ❌ MultiSelectList → ✅ SelectList (since one supplier only)
            ViewBag.Suppliers = new SelectList(
                _context.Suppliers.Where(s => s.IsActive),
                "SupplierId",
                "CompanyName"
            );

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string Name,
            string Description,
            int SupplierId,
            decimal SupplierPrice,
            string Unit)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                TempData["ErrorMessage"] = "Product name is required.";
                return RedirectToAction(nameof(Create));
            }

            var product = new Product
            {
                Name = Name,
                Description = Description,
                Unit = Unit,
                ProductSuppliers = new List<ProductSupplier>()
        {
            new ProductSupplier
            {
                SupplierId = SupplierId,
                Price = SupplierPrice
            }
        }
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }





        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.ProductSuppliers)
                    .ThenInclude(ps => ps.Supplier)
                .AsNoTracking() 
                .FirstOrDefaultAsync(m => m.ProductId == id);


            if (product == null) return NotFound();

            return View(product);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.ProductSuppliers)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            ViewBag.DescriptionList = new SelectList(new List<string>
            {
                "Sprinkler Heads Material",
                "Black Iron Pipe Material",
                "Welded Fitting Material",
                "Standard Fitting Material",
                "Reducer Fitting Material",
                "Grooved Coupling Material",
                "Polyvinyl Chloride Pipe (PVC) pipe",
                "Electrical Material",
                "Bolt Nut & Washer",
                "Valves Material"
            }, product.Description);

            var currentSupplierId = product.ProductSuppliers.FirstOrDefault()?.SupplierId;

            ViewBag.Suppliers = new SelectList(
                _context.Suppliers.Where(s => s.IsActive),
                "SupplierId",
                "CompanyName",
                currentSupplierId
            );

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string name,
            string description,
            string unit,
            int supplierId,
            decimal supplierPrice
        )
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns if validation fails
                ViewBag.Suppliers = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", supplierId);
                ViewBag.DescriptionList = new SelectList(_context.Products.Select(p => p.Description).Distinct());
                return View();
            }

            var product = await _context.Products
                .Include(p => p.ProductSuppliers)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            // Update basic product fields
            product.Name = name;
            product.Description = description;
            product.Unit = unit;

            // Enforce one supplier per product
            var existingSupplier = product.ProductSuppliers.FirstOrDefault();
            if (existingSupplier != null)
            {
                existingSupplier.SupplierId = supplierId;
                existingSupplier.Price = supplierPrice;
            }
            else
            {
                product.ProductSuppliers.Add(new ProductSupplier
                {
                    SupplierId = supplierId,
                    Price = supplierPrice
                });
            }

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Product '{name}' updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists contact support.");
                return View(product);
            }
        }



        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductSuppliers)
                .Include(p => p.PurchaseItems)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            // Check if product is used in any purchases
            if (product.PurchaseItems != null && product.PurchaseItems.Any())
            {
                TempData["ErrorMessage"] = "This product cannot be deleted because it is already used in customer purchases.";
                return RedirectToAction(nameof(Index));
            }

            // Remove related ProductSuppliers
            if (product.ProductSuppliers.Any())
            {
                _context.ProductSuppliers.RemoveRange(product.ProductSuppliers);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Product '{product.Name}' deleted successfully!";
            return RedirectToAction(nameof(Index));
        }



        // AJAX: Update supplier-specific price (NO OldPrice)
        [HttpPost]
        public async Task<IActionResult> UpdatePrice([FromBody] SupplierPriceUpdateDto request)
        {
            // Validate
            if (request == null) return BadRequest();

            var productSupplier = await _context.ProductSuppliers
                .FirstOrDefaultAsync(ps => ps.ProductId == request.ProductId && ps.SupplierId == request.SupplierId);

            if (productSupplier == null) return NotFound();

            productSupplier.Price = request.Price;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        // Local DTO to avoid any conflicts with other UpdatePriceRequest classes
        public class SupplierPriceUpdateDto
        {
            public int ProductId { get; set; }
            public int SupplierId { get; set; }
            public decimal Price { get; set; }
        }
    }
}
