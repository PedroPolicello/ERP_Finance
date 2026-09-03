using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Tests.Fakes;

public class FakeOrderItemRepository : IOrderItemRepository
{
    private readonly List<OrderItem> _orderItems = new();

    public IReadOnlyList<OrderItem> OrderItems =>
        _orderItems.AsReadOnly();

    public bool ShouldFailOnAdd { get; set; }

    public bool AddToRepository(OrderItem orderItem)
    {
        ArgumentNullException.ThrowIfNull(orderItem);

        if (ShouldFailOnAdd)
            return false;

        _orderItems.Add(orderItem);

        return true;
    }

    public OrderItem? GetOrderItemById(Guid id)
    {
        return _orderItems.FirstOrDefault(orderItem => orderItem.Id == id);
    }

    public bool RemoveFromRepository(OrderItem orderItem)
    {
        if(orderItem is null)
            return false;
        
        return _orderItems.Remove(orderItem);
    }
}