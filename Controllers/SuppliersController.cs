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
    public class SuppliersController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public SuppliersController(PJInstallationDbContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            int pageSize = 5;

            var suppliersQuery = _context.Suppliers.AsQueryable();

            // 🔍 Apply search
            if (!string.IsNullOrEmpty(searchString))
            {
                suppliersQuery = suppliersQuery.Where(s =>
                    s.CompanyName.Contains(searchString) ||
                    s.ContactPerson.Contains(searchString) ||
                    s.Email.Contains(searchString) ||
                    s.Phone.Contains(searchString));
            }

            var totalSuppliers = await suppliersQuery.CountAsync();

            // 📄 Apply pagination
            var suppliers = await suppliersQuery
                .OrderBy(s => s.CompanyName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalSuppliers / (double)pageSize);
            ViewBag.TotalSuppliers = totalSuppliers;
            ViewBag.SearchString = searchString;

            return View(suppliers);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(
            string companyName,
            string contactPerson,
            string phone,
            string email,
            bool isActive = true) // Default to true if not provided
        {
            try
            {
                var supplier = new Supplier
                {
                    CompanyName = companyName,
                    ContactPerson = contactPerson,
                    Phone = phone,
                    Email = email,
                    IsActive = isActive
                };

                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Supplier '{companyName}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the detailed error
                System.Diagnostics.Debug.WriteLine($"Supplier create error: {ex}");

                // User-friendly error message
                TempData["ErrorMessage"] = "Failed to create supplier. Please check the data and try again.";
                return View();
            }
        }



        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .Include(s => s.ProductSuppliers)            
                .ThenInclude(ps => ps.Product)               
                .FirstOrDefaultAsync(m => m.SupplierId == id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }


        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string companyName, string contactPerson, string phone, string email, bool isActive)
        {
            try
            {
                var supplier = await _context.Suppliers.FindAsync(id);
                if (supplier == null)
                {
                    return NotFound();
                }

                supplier.CompanyName = companyName;
                supplier.ContactPerson = contactPerson;
                supplier.Phone = phone;
                supplier.Email = email;
                supplier.IsActive = isActive;

                _context.Update(supplier);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(new Supplier
                {
                    SupplierId = id,
                    CompanyName = companyName,
                    ContactPerson = contactPerson,
                    Phone = phone,
                    Email = email,
                    IsActive = isActive
                });
            }
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM Suppliers WHERE SupplierId = {id}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to force delete. Error: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }




        private bool SupplierExists(int id)
        {
            return _context.Suppliers.Any(e => e.SupplierId == id);
        }
    }
}
