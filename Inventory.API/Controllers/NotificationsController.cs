using Inventory.Shared.Data;
using Inventory.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController(InventoryDbContext dbContext) : ControllerBase
    {
        private readonly InventoryDbContext _dbContext = dbContext;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryNotification>>> GetNotifications()
        {
            var notifications = await _dbContext
                .Notifications.OrderByDescending(n => n.Timestamp)
                .ToListAsync();

            return Ok(notifications);
        }
    }
}
