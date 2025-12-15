using Microsoft.AspNetCore.Mvc;
using PJ_P_Installation_Management_System.Data;
using PJ_P_Installation_Management_System.Models;
using System;
using System.Linq;

namespace PJ_P_Installation_Management_System.Controllers
{
    public class LoginController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public LoginController(PJInstallationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == model.Username && u.Password == model.Password);

            if (user == null)
            {
                ViewBag.LoginError = "Incorrect username or password.";
                return View(model);
            }

            // Proceed with login (e.g. set session or cookie)
            return RedirectToAction("Index", "Home");
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }

    public class DevController : Controller
    {
        private readonly PJInstallationDbContext _context;

        public DevController(PJInstallationDbContext context)
        {
            _context = context;
        }

        public IActionResult Seed()
        {
            UserSeeder.SeedAdminUser(_context);
            return Content("Admin user created.");
        }
    }

}
