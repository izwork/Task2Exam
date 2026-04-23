using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenfieldLocal.Data;
using GreenfieldLocal.Models;
using System.Security.Claims;
using Newtonsoft.Json.Linq;

namespace GreenfieldLocal.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (User.IsInRole("Admin"))
            {
                var allOrders = await _context.Orders.Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Products)
                    .ToListAsync();
                return View(allOrders);
            }
            else if (User.IsInRole("Supplier"))
            {
                var supplierProducts = await _context.Products.Where(p => p.Suppliers.UserId == userId)
                    .Select(p => p.ProductsId)
                    .ToListAsync(); // Find all supplier products.
                var supplierOrders = await _context.OrderProducts.Where(op => supplierProducts.Contains(op.ProductsId))
                    .Include(op => op.Orders)
                    .Include(op => op.Products)
                    .ToListAsync(); // Use the supplier products to find supplier orders.
                return View(supplierOrders.Select(op => op.Orders).Distinct().ToList());
            }
            else
            {
                var userOrders = await _context.Orders.Where(o => o.UserId == userId)
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Products)
                    .ToListAsync();
                return View(userOrders);
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.OrderProducts
                .Where(x => x.OrdersId == id)
                .Include(x => x.Orders)
                .Include(x => x.Products)
                .ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        public IActionResult Create(int basketId)
        {
            ViewBag.basketId = basketId;
            ViewBag.BasketId = basketId; // provide both casings for views

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return View();
            }

            // Load basket products and calculate totals so the checkout page can show an order summary
            var basket = _context.Basket.FirstOrDefault(x => x.BasketId == basketId && x.UserId == userId && x.Status);
            if (basket == null)
            {
                return View();
            }

            var basketProducts = _context.BasketProducts
                .Where(x => x.BasketId == basketId)
                .Include(x => x.Products)
                .ToList();

            decimal subtotal = 0m;
            foreach (var bp in basketProducts)
            {
                subtotal += bp.Products.Price * bp.Quantity;
            }

            var orderCount = _context.Orders.Count(x => x.UserId == userId);
            decimal discount = 0m;
            if (orderCount >= 5)
            {
                discount = subtotal * 0.10m;
            }

            decimal total = subtotal - discount;

            ViewBag.Subtotal = subtotal;
            ViewBag.Discount = discount;
            ViewBag.Total = total;
            ViewBag.OrderCount = orderCount;
            ViewBag.BasketProducts = basketProducts;

            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrdersId,Delivery,Collection,DeliveryType,CollectionDate")] Orders orders, int basketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                ViewBag.BasketId = basketId;
                return View(orders);
            }

            // Assign values.
            orders.UserId = userId;
            ModelState.Remove("UserId");

            orders.OrderDate = DateOnly.FromDateTime(DateTime.Today);
            ModelState.Remove("OrderDate");

            orders.OrderTrackingStatus = "Pending";
            ModelState.Remove("OrderTrackingStatus");

            var basket = await _context.Basket
                .FirstOrDefaultAsync(x => x.BasketId == basketId && x.UserId == userId && x.Status);

            if (basket == null)
            {
                return NotFound();
            }

            // Get basket products.
            var basketProducts = await _context.BasketProducts
                .Where(x => x.BasketId == basketId)
                .Include(x => x.Products)
                .ToListAsync();

            if (!basketProducts.Any())
            {
                ModelState.AddModelError("", "Your basket is empty.");
                ViewBag.BasketId = basketId;
                return View(orders);
            }

            // Calculate subtotal
            decimal subtotal = 0.00m;
            foreach (var basketProduct in basketProducts)
            {
                var productTotal = basketProduct.Products.Price * basketProduct.Quantity;
                subtotal = productTotal + subtotal;
            }

            var orderCount = await _context.Orders.CountAsync(x => x.UserId == userId);

            // Discount
            decimal discount = 0m;

            if (orderCount >= 5)
            {
                discount = subtotal * 0.10m;
            }

            orders.Subtotal = subtotal - discount;

            ModelState.Remove("Subtotal");

            // Delivery and collection validation
            if (!orders.Collection && !orders.Delivery)
            {
                ModelState.AddModelError("Delivery", "Must choose Collection or Delivery.");
            }

            if (orders.Collection)
            {
                ModelState.Remove("DeliveryType");

                if (orders.CollectionDate == null)
                {
                    ModelState.AddModelError("CollectionDate", "Collection date is required.");
                }
                else
                { 
                    var earliestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));

                    if (orders.CollectionDate.Value < earliestDate)
                    {
                        ModelState.AddModelError("CollectionDate", "Collection date must be at least 2 days from today.");
                    }
                }
            }

            if (orders.Delivery)
            {
                ModelState.Remove("CollectionDate");

                if (string.IsNullOrWhiteSpace(orders.DeliveryType))
                {
                    ModelState.AddModelError("DeliveryType", "Delivery type is required.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.BasketId = basketId;
                return View(orders);
            }

            // Create order
            _context.Orders.Add(orders);
            await _context.SaveChangesAsync();

            foreach (var basketProduct in basketProducts)
            {
                if (basketProduct.Products.Stock < basketProduct.Quantity)
                {
                    ModelState.AddModelError("", $"Not enough stock for {basketProduct.Products.ProductName}");
                    ViewBag.BasketId = basketId;
                    return View(orders);
                }

                var orderProduct = new OrderProducts
                {
                    OrdersId = orders.OrdersId,
                    ProductsId = basketProduct.ProductsId,
                    Quantity = basketProduct.Quantity
                };

                _context.OrderProducts.Add(orderProduct);
                
                basketProduct.Products.Stock -= basketProduct.Quantity;
            }

            // Close basket
            basket.Status = false;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrdersId,UserId,Subtotal,Delivery,Collection,DeliveryType,OrderTrackingStatus,CollectionDate,OrderDate")] Orders orders)
        {
            if (id != orders.OrdersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.OrdersId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrdersId == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrdersId == id);
        }
    }
}
