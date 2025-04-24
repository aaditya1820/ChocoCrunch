using Microsoft.AspNetCore.Mvc;
using Aaditya.Data;
using Aaditya.Models;
using System.Linq;

namespace Aaditya.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.User.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Profile", new { id = user.uID });
            }
            return View(user);
        }

        public IActionResult Edit(int id)
        {
            var user = _context.User.Find(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                _context.User.Update(user);
                _context.SaveChanges();
                return RedirectToAction("Profile", new { id = user.uID });
            }
            return View(user);
        }

        public IActionResult Profile(int id)
        {
            var user = _context.User.FirstOrDefault(u => u.uID == id);
            if (user == null)
                return NotFound();

            return View(user);
        }
    }
}
