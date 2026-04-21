// GET: Products/Create
[Authorize(Roles = "Supplier, Admin, Developer")]
public async Task<IActionResult> Create()
{
    if (User.IsInRole("Supplier"))
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.UserId == userId);
        if (supplier == null) return NotFound();

        ViewData["SuppliersId"] = new SelectList(new[] { supplier }, "SuppliersId", "SupplierName", supplier.SuppliersId);
        ViewBag.SupplierName = supplier.SupplierName;
    }
    else
    {
        ViewData["SuppliersId"] = new SelectList(_context.Suppliers, "SuppliersId", "SupplierName");
    }

    return View();
}