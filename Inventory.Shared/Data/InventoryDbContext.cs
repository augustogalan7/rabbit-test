using Inventory.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Shared.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<InventoryNotification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<InventoryNotification>().HasKey(n => n.Id);
            modelBuilder.Entity<InventoryNotification>().Property(n => n.Id).ValueGeneratedOnAdd();
        }
    }
}
