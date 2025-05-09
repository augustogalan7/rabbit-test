namespace Inventory.Shared.Models;

public class InventoryNotification
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
