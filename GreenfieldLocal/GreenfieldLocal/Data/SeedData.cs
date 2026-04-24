using GreenfieldLocal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace GreenfieldLocal.Data
{
    public class SeedData
    {
        public static async Task SeedUsersAndRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seeded my roles
            string[] roleNames = { "Admin", "Supplier", "Standard", "Developer" };
            foreach (string roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName); // Check if the role already exists.
                if (!roleExists)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }

            // Seeding users and assigning roles, one for each type of user
            // Adding the Admin User
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(adminUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Adding Supplier User #1
            var supplierUser = await userManager.FindByEmailAsync("supplier@example.com");
            if (supplierUser == null)
            {
                supplierUser = new IdentityUser { UserName = "supplier@example.com", Email = "supplier@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(supplierUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(supplierUser, "Supplier"))
            {
                await userManager.AddToRoleAsync(supplierUser, "Supplier");
            }

            // Adding Supplier User #2
            var supplierUser2 = await userManager.FindByEmailAsync("supplier2@example.com");
            if (supplierUser2 == null)
            {
                supplierUser2 = new IdentityUser { UserName = "supplier2@example.com", Email = "supplier2@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(supplierUser2, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(supplierUser2, "Supplier"))
            {
                await userManager.AddToRoleAsync(supplierUser2, "Supplier");
            }

            // Adding Supplier User #3 
            var supplierUser3 = await userManager.FindByEmailAsync("supplier3@example.com");
            if (supplierUser3 == null)
            {
                supplierUser3 = new IdentityUser { UserName = "supplier3@example.com", Email = "supplier3@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(supplierUser3, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(supplierUser3, "Supplier"))
            {
                await userManager.AddToRoleAsync(supplierUser3, "Supplier");
            }

            // Adding the Standard User
            var standardUser = await userManager.FindByEmailAsync("user@example.com");
            if (standardUser == null)
            {
                standardUser = new IdentityUser {UserName = "user@example.com", Email = "user@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(standardUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(standardUser, "Standard"))
            {
                await userManager.AddToRoleAsync(standardUser, "Standard");
            }

            // Adding the Developer User
            var devUser = await userManager.FindByEmailAsync("dev@example.com");
            if (devUser == null)
            {
                devUser = new IdentityUser { UserName = "dev@example.com", Email = "dev@example.com", EmailConfirmed = true };
                await userManager.CreateAsync(devUser, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(devUser, "Developer"))
            {
                await userManager.AddToRoleAsync(devUser, "Developer");
            }
        }

        public static async Task SeedSuppliers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Find the User by Email
            var SupplierUser1 = await userManager.FindByEmailAsync("supplier@example.com");
            var SupplierUser2 = await userManager.FindByEmailAsync("supplier2@example.com");
            var SupplierUser3 = await userManager.FindByEmailAsync("supplier3@example.com");

            if (SupplierUser1 == null || SupplierUser2 == null || SupplierUser3 == null)
            {
                throw new Exception("Supplier user not found.");
            }


            // Prevents duplicate seeding
            if (context.Suppliers.Any())
                return;

            var suppliers = new List<Suppliers>
            {
                new Suppliers()
                {
                    SupplierName = "Fresh Farm Produce",
                    SupplierEmail = "contact@freshfarm.co.uk",
                    SupplierInformation = "Local farm supplying organic fruits and vegetables.",
                    UserId = SupplierUser1.Id
                },
                new Suppliers()
                {
                    SupplierName = "Big Farm UK",
                    SupplierEmail =  "sales@bigfarm.co.uk",
                    SupplierInformation = "Biggest UK farm supplying fresh fruits and vegetables.",
                    UserId = SupplierUser2.Id
                },
                new Suppliers()
                {
                    SupplierName = "Sandwell Farm",
                    SupplierEmail = "farm@sandwell.ac.uk",
                    SupplierInformation = "Local farm in Sandwell, supplying organic fruits and vegetables.",
                    UserId = SupplierUser3.Id
                }
            };

            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();
        }

        public static async Task SeedProducts(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Find the Supplier
            var SandwellFarm = await context.Suppliers.FirstOrDefaultAsync(x => x.SupplierName == "Sandwell Farm");
            var BigFarmUK = await context.Suppliers.FirstOrDefaultAsync(x => x.SupplierName == "Big Farm UK");
            var FreshFarm = await context.Suppliers.FirstOrDefaultAsync(x => x.SupplierName == "Fresh Farm Produce");

            if (SandwellFarm == null || BigFarmUK == null || FreshFarm == null)
            {
                throw new Exception("Supplier not found");
            }

            if (!context.Products.Any())
            {
                var products = new List<Products>
                {
                    new Products
                    {
                        ProductName = "Apple",
                        Stock = 100,
                        Price = 0.50m,
                        SuppliersId = SandwellFarm.SuppliersId,
                        ImagePath = "/images/apple.png"
                    },
                     new Products
                    {
                        ProductName = "Banana",
                        Stock = 120,
                        Price = 0.60m,
                        SuppliersId = BigFarmUK.SuppliersId,
                        ImagePath = "/images/banana.png"
                    },
                      new Products
                    {
                        ProductName = "Orange",
                        Stock = 90,
                        Price = 0.55m,
                        SuppliersId = FreshFarm.SuppliersId,
                        ImagePath = "/images/orange.jpg"
                    },
                       new Products
                    {
                        ProductName = "Bread",
                        Stock = 60,
                        Price = 1.20m,
                        SuppliersId = SandwellFarm.SuppliersId,
                        ImagePath = "/images/bread.jpg"
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
