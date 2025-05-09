using System;
using System.Threading.Tasks;
using Inventory.Shared.Data;
using Inventory.Shared.Messages;
using Inventory.Shared.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Notification.Service.Consumers
{
    public class InventoryConsumer(InventoryDbContext dbContext, ILogger<InventoryConsumer> logger)
        : IConsumer<ProductCreatedMessage>,
            IConsumer<ProductUpdatedMessage>,
            IConsumer<ProductDeletedMessage>
    {
        private readonly InventoryDbContext _dbContext = dbContext;
        private readonly ILogger<InventoryConsumer> _logger = logger;

        public async Task Consume(ConsumeContext<ProductCreatedMessage> context)
        {
            await ProcessNotification(context.Message, "created");
        }

        public async Task Consume(ConsumeContext<ProductUpdatedMessage> context)
        {
            await ProcessNotification(context.Message, "updated");
        }

        public async Task Consume(ConsumeContext<ProductDeletedMessage> context)
        {
            await ProcessNotification(context.Message, "deleted");
        }

        private async Task ProcessNotification(dynamic message, string eventType)
        {
            var notification = new InventoryNotification
            {
                EventType = eventType,
                ProductName = message.ProductName,
                Quantity = message.Quantity,
                Details = message.Details,
                Timestamp = DateTime.UtcNow,
            };

            _logger.LogInformation(
                "Processing {EventType} notification for product {ProductName}: {Details}",
                notification.EventType,
                notification.ProductName,
                notification.Details
            );

            try
            {
                _dbContext.Notifications.Add(notification);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(
                    "Successfully processed {EventType} notification for product {ProductName}",
                    notification.EventType,
                    notification.ProductName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing {EventType} notification for product {ProductName}",
                    notification.EventType,
                    notification.ProductName
                );
                throw;
            }
        }
    }
}
