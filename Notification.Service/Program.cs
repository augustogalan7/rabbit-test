using Inventory.Shared.Data;
using Inventory.Shared.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Notification.Service.Configuration;
using Notification.Service.Consumers;

var builder = Host.CreateDefaultBuilder(args);

// Add services to the container.
builder.ConfigureServices(
    (hostContext, services) =>
    {
        services.Configure<RabbitMQConfig>(hostContext.Configuration.GetSection("RabbitMQ"));

        services.AddDbContext<InventoryDbContext>(options =>
            options.UseSqlite("Data Source=../SharedData/inventory.db")
        );

        services.AddMassTransit(x =>
        {
            x.AddConsumer<InventoryConsumer>();

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

                    cfg.ReceiveEndpoint(
                        "inventory-notifications-queue",
                        e =>
                        {
                            e.ConfigureConsumer<InventoryConsumer>(context);
                            // e.Bind(
                            //     "inventory_exchange",
                            //     s =>
                            //     {
                            //         s.ExchangeType = "direct";
                            //         s.RoutingKey = "inventory.*";
                            //     }
                            // );
                        }
                    );

                    // Configure retry and circuit breaker
                    cfg.UseMessageRetry(r =>
                    {
                        r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
                        r.Handle<Exception>();
                    });

                    cfg.UseKillSwitch(options =>
                        options
                            .SetActivationThreshold(10)
                            .SetTripThreshold(0.3)
                            .SetRestartTimeout(m: 1)
                    );

                    cfg.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                        cb.TripThreshold = 15;
                        cb.ActiveThreshold = 10;
                        cb.ResetInterval = TimeSpan.FromMinutes(1);
                    });
                }
            );
        });
    }
);

var host = builder.Build();

// Ensure database is created
using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    dbContext.Database.EnsureCreated();
}

await host.RunAsync();
