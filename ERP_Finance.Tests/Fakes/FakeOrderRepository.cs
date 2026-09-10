using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Tests.Fakes;

public class FakeOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();
    public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

    public bool ShouldFailOnAdd { get; set; }

    public bool AddToRepository(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        if (ShouldFailOnAdd)
            return false;

        _orders.Add(order);

        return true;
    }

    public Order? GetOrderById(Guid id)
    {
        return _orders.FirstOrDefault(order => order.Id == id);
    }
}