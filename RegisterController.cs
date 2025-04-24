using Aaditya.Data;
using Microsoft.AspNetCore.Mvc;

namespace Aaditya.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{email}")]
        public IActionResult GetUserByEmail(string email)
        {
            var user = _context.Register.FirstOrDefault(u => u.Email == email);
            if (user == null) return NotFound();

            return Ok(user);
        }
    }
}
