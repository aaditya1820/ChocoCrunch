using Aaditya.Data;
using Aaditya.Models;
using Aaditya.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aaditya.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private static Dictionary<string, string> otpStorage = new();

        public AccountController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // 1. Registration / Login==========================================================================================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _context.Register.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered");
                return View(model);
            }

            var user = new Register
            {
                Name = model.Name,
                Email = model.Email,
                Contact = model.Contact,
                Password = model.Password  // Password stored as plain text (Needs hashing for security)
            };

            _context.Add(user);
            await _context.SaveChangesAsync();

            _context.Add(new Login
            {
                Email = model.Email,
                Password = model.Password
            });
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check for default admin login================================================================


            if (model.Email == "chococrunch@gmail.com" && model.Password == "Admin@123")
            {
                return Redirect("~/Ready-Bootstrap-Dashboard-master/index.html");
            }

            var user = await _context.Login.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View(model);
            }

            return Redirect("~/chocolux-master/CUSTOMER_PANEL.html");
        }


        //====================2. Forgot Password====================
        public IActionResult ForgotPassword()
        {
            return View("~/Views/Account/ForgotPassword.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("", "Email is required");
                return View(model);
            }

            var user = await _context.Register.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with this email");
                return View(model);
            }

            string otp = new Random().Next(100000, 999999).ToString();
            otpStorage[model.Email] = otp;

            await _emailSender.SendEmailAsync(model.Email, "Password Reset OTP", $"Your OTP is: {otp}");

            TempData["Email"] = model.Email;
            return RedirectToAction("VerifyOTP");
        }

        // 3. OTP Verification==============================================================================================
        public IActionResult VerifyOTP()
        {
            return View("~/Views/Account/VerifyOTP.cshtml");
        }

        [HttpPost]
        public IActionResult VerifyOTP(OTPModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.OTP))
            {
                ModelState.AddModelError("", "Email and OTP are required");
                return View(model);
            }

            if (otpStorage.TryGetValue(model.Email, out string correctOtp) && correctOtp == model.OTP)
            {
                otpStorage.Remove(model.Email);
                TempData["Email"] = model.Email;
                return RedirectToAction("ResetPassword");
            }

            ModelState.AddModelError("", "Invalid OTP");
            return View(model);
        }

        // 4. Password Reset================================================================================================
        public IActionResult ResetPassword()
        {
            return View("~/Views/Account/ResetPassword.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("", "Email and new password are required");
                return View();
            }

            var user = await _context.Register.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.Password = newPassword;  // Password stored as plain text
                await _context.SaveChangesAsync();

                // Also update the Login table
                var loginUser = await _context.Login.FirstOrDefaultAsync(l => l.Email == email);
                if (loginUser != null)
                {
                    loginUser.Password = newPassword;
                    await _context.SaveChangesAsync();
                }

                TempData["Message"] = "Password reset successfully!";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "User not found");
            return View();
        }




        // ======== 5. Registration Search ===================================
        public async Task<IActionResult> Reg(string search)
        {
            var users = _context.Register.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(u =>
                    u.Name.Contains(search) ||
                    u.Email.Contains(search));
            }

            var result = await users.ToListAsync();
            return View(result);
        }

        // ======== 6. Seller Search =========================================
        public async Task<IActionResult> Seller(string search)
        {
            var sellers = _context.Seller.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                sellers = sellers.Where(s =>
                    s.SellerName.Contains(search) ||
                    s.SellerEmail.Contains(search));
            }

            var result = await sellers.ToListAsync();
            return View(result);
        }

        // ======== 7. Chocolate Search ======================================
        public async Task<IActionResult> Choco(string searchString)
        {
            var chocolates = _context.Chocolate.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                chocolates = chocolates.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.Brand.Contains(searchString) ||
                    c.Flavour.Contains(searchString));
            }

            var result = await chocolates.ToListAsync();
            return View(result);
        }

        //========8. Logout=========================================

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


        //========9. Add Seller=========================================

        // GET: /Account/Seller_Add
        public IActionResult Seller_Add()
        {
            return View(_context.Seller.ToList());
        }

        // GET: /Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Seller seller)
        {
            if (ModelState.IsValid)
            {
                _context.Seller.Add(seller);
                _context.SaveChanges();
                return RedirectToAction("Seller_Add");
            }
            return View(seller);
        }

        // GET: /Account/Edit/5
        public IActionResult Edit(int id)
        {
            var seller = _context.Seller.Find(id);
            if (seller == null)
            {
                return NotFound();
            }
            return View(seller);
        }

        // POST: /Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Seller seller)
        {
            if (ModelState.IsValid)
            {
                _context.Seller.Update(seller);
                _context.SaveChanges();
                return RedirectToAction("Seller_Add");
            }
            return View(seller);
        }

        // GET: /Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seller = await _context.Seller.FirstOrDefaultAsync(s => s.SellerID == id);
            if (seller == null)
            {
                return NotFound();
            }

            return View(seller);
        }

        // POST: /Account/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int SellerID)
        {
            var seller = await _context.Seller.FindAsync(SellerID);
            if (seller != null)
            {
                try
                {
                    _context.Seller.Remove(seller);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Seller deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "Error deleting seller. It might be referenced in another table.";
                }
            }
            else
            {
                TempData["Error"] = "Seller not found!";
            }

            return RedirectToAction(nameof(Seller_Add));
        }

        //========10. Add Chocolate=========================================

        public IActionResult Index1()
        {
            return View(_context.Chocolate.ToList());
        }

        public IActionResult Create1()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create1(Choco chocolate)
        {
            if (ModelState.IsValid)
            {
                _context.Chocolate.Add(chocolate);
                _context.SaveChanges();
                return RedirectToAction("Index1");
            }
            return View(chocolate);
        }

        public IActionResult Edit1(int id)
        {
            var chocolate = _context.Chocolate.Find(id);
            if (chocolate == null)
            {
                return NotFound();
            }
            return View(chocolate);
        }

        [HttpPost]
        public IActionResult Edit1(Choco chocolate)
        {
            if (ModelState.IsValid)
            {
                _context.Chocolate.Update(chocolate);
                _context.SaveChanges();
                return RedirectToAction("Index1");
            }
            return View(chocolate);
        }

        public async Task<IActionResult> Delete1(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chocolate = await _context.Chocolate.FirstOrDefaultAsync(c => c.ChocolateID == id);
            if (chocolate == null)
            {
                return NotFound();
            }

            return View(chocolate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed1(int id)
        {
            var chocolate = await _context.Chocolate.FindAsync(id);
            if (chocolate != null)
            {
                try
                {
                    _context.Chocolate.Remove(chocolate);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Chocolate deleted successfully!";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "Error deleting chocolate. It might be referenced in another table.";
                }
            }
            else
            {
                TempData["Error"] = "Chocolate not found!";
            }

            return RedirectToAction(nameof(Index1));
        }

    }
}





