using GreenfieldLocal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GreenfieldLocal.Controllers
{
    [Authorize(Roles = "Supplier")]
    public class SupplierDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SupplierDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == userId);

            if (supplier == null)
            {
                return NotFound();
            }

            var products = await _context.Products.Where(x => x.SuppliersId == supplier.SuppliersId).ToListAsync();

            var orders = await _context.Orders.Include(o => o.OrderProducts)
                                              .ThenInclude(op => op.Products)
                                              .Where(o => o.OrderProducts.Any(op => op.Products.SuppliersId == supplier.SuppliersId))
                                              .ToListAsync();

            ViewBag.TotalProducts = products.Count;
            ViewBag.LowStockCount = products.Count(x => x.Stock <= 5);
            ViewBag.RecentOrders = orders;

            return View(products);
        }
    }
}
