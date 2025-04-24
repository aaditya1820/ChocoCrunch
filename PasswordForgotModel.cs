using Microsoft.AspNetCore.Mvc;

namespace Aaditya.Models
{
    public class PasswordForgotModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
