namespace Inventory.Shared.Messages;

public class ProductCreatedMessage
{
    public string EventType { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Details { get; set; } = string.Empty;
}
