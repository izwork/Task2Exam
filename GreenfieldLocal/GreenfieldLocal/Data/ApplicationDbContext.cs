using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GreenfieldLocal.Models;

namespace GreenfieldLocal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<GreenfieldLocal.Models.Suppliers> Suppliers { get; set; } = default!;
        public DbSet<GreenfieldLocal.Models.Products> Products { get; set; } = default!;
        public DbSet<GreenfieldLocal.Models.Orders> Orders { get; set; } = default!;
        public DbSet<GreenfieldLocal.Models.OrderProducts> OrderProducts { get; set; } = default!;
        public DbSet<GreenfieldLocal.Models.BasketProducts> BasketProducts { get; set; } = default!;
        public DbSet<GreenfieldLocal.Models.Basket> Basket { get; set; } = default!;
    }
}
