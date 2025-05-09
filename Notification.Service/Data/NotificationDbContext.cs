using Inventory.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Notification.Service.Data
{
    public class NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : DbContext(options)
    {
        public DbSet<InventoryNotification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InventoryNotification>().HasKey(n => n.Id);
            modelBuilder.Entity<InventoryNotification>().Property(n => n.Id).ValueGeneratedOnAdd();
        }
    }
}
