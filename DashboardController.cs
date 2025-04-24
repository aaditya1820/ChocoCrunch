using Microsoft.AspNetCore.Mvc;
using Aaditya.Data; // Make sure this namespace matches your DbContext namespace

namespace ChocoCrunch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("counts")]
        public IActionResult GetDashboardCounts()
        {
            var customerCount = _context.Register.Count();
            var sellerCount = _context.Seller.Count();
            var chocolateCount = _context.Chocolate.Count();

            return Ok(new
            {
                customerCount,
                sellerCount,
                chocolateCount
            });
        }
    }
}
