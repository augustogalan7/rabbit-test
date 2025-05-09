using Inventory.API.Configuration;
using Inventory.Shared.Data;
using Inventory.Shared.Messages;
using Inventory.Shared.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure RabbitMQ
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));

// Configure DbContext using configuration from appsettings.json
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq(
        (context, cfg) =>
        {
            var config = context.GetRequiredService<IOptions<RabbitMQConfig>>().Value;
            cfg.Host(
                config.HostName,
                config.VirtualHost,
                h =>
                {
                    h.Username(config.UserName);
                    h.Password(config.Password);
                }
            );

            // Configurar el exchange
            cfg.Message<ProductCreatedMessage>(e => e.SetEntityName("inventory_exchange"));
            cfg.Message<ProductUpdatedMessage>(e => e.SetEntityName("inventory_exchange"));
            cfg.Message<ProductDeletedMessage>(e => e.SetEntityName("inventory_exchange"));

            // Configurar la publicación de mensajes
            cfg.Publish<ProductCreatedMessage>(e =>
            {
                e.ExchangeType = "direct";
                e.AutoDelete = false;
                e.Durable = true;
                e.BindQueue(
                    "inventory_exchange",
                    "inventory-notifications-queue",
                    s =>
                    {
                        s.ExchangeType = "direct";
                        s.RoutingKey = "inventory.created";
                    }
                );
            });

            cfg.Publish<ProductUpdatedMessage>(e =>
            {
                e.ExchangeType = "direct";
                e.AutoDelete = false;
                e.Durable = true;
                e.BindQueue(
                    "inventory_exchange",
                    "inventory-notifications-queue",
                    s =>
                    {
                        s.ExchangeType = "direct";
                        s.RoutingKey = "inventory.updated";
                    }
                );
            });

            cfg.Publish<ProductDeletedMessage>(e =>
            {
                e.ExchangeType = "direct";
                e.AutoDelete = false;
                e.Durable = true;
                e.BindQueue(
                    "inventory_exchange",
                    "inventory-notifications-queue",
                    s =>
                    {
                        s.ExchangeType = "direct";
                        s.RoutingKey = "inventory.deleted";
                    }
                );
            });
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
}

app.Run();
