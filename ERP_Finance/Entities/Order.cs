using ERP_Finance.Types;

namespace ERP_Finance.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public int OrderNumber { get; private set; }
    public Guid TabId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? Note { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    public Order(int orderNumber, Guid tabId, OrderStatus status = OrderStatus.SentToKitchen, string? note = null)
    {
        ValidateOrderNumber(orderNumber);
        ValidateTabId(tabId);

        Id = Guid.NewGuid();
        OrderNumber = orderNumber;
        TabId = tabId;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        Note = note;
        OrderItems = new List<OrderItem>();
    }

    public void AddOrderItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if(item.OrderId != Id)
            throw new ArgumentException("Order item does not belong to this order.");

        OrderItems.Add(item);
    }

    public void RemoveOrderItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        OrderItems.Remove(item);
    }

    public OrderItem? GetOrderItem(Guid orderItemId)
    {
        return OrderItems.FirstOrDefault(i => i.Id == orderItemId);
    }

    private void ValidateOrderNumber(int number)
    {
        if (number <= 0)
            throw new ArgumentException("Order number must be a positive integer.");
    }

    private void ValidateTabId(Guid tabId)
    {
        if (tabId == Guid.Empty)
            throw new ArgumentException("Tab ID must be a valid GUID.");
    }
}
