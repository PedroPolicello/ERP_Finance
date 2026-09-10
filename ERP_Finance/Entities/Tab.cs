using ERP_Finance.Types;

namespace ERP_Finance.Entities;

public class Tab
{
    public Guid Id { get; private set; }
    public int TabNumber { get; private set; }
    public TabStatus Status { get; private set; }
    public ServiceType ServiceType { get; private set; }
    public int? TableNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public ICollection<Order> Orders { get; private set; } = new List<Order>();
    public string? Note { get; private set; }
    public bool IsOpen => Status != TabStatus.Closed && Status != TabStatus.Canceled;

    public Tab(int tabNumber, ServiceType serviceType, int? tableNumber = null, string? note = null)
    {
        ValidateTabNumber(tabNumber);
        ValidateServiceType(serviceType);

        Id = Guid.NewGuid();
        TabNumber = tabNumber;
        Status = TabStatus.Open;
        ServiceType = serviceType;
        TableNumber = tableNumber;
        CreatedAt = DateTime.UtcNow;
        ClosedAt = null;
        Note = note;
    }

    public void AddOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if(!IsOpen)
            throw new InvalidOperationException("Only open tabs can have orders added.");

        if(Orders.Contains(order))
            throw new ArgumentException("Order already exists in the tab.", nameof(order));

        Orders.Add(order);
    }

    public void RemoveOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        if (!IsOpen)
            throw new InvalidOperationException("Only open tabs can have orders removed.");

        if (!Orders.Contains(order))
            throw new ArgumentException("Order does not exist in the tab.", nameof(order));

        Orders.Remove(order);
    }

    public Order? GetOrderById(Guid orderId)
    {
        return Orders.FirstOrDefault(o => o.Id == orderId);
    }

    public List<Order> GetAllOrders()
    {
        return Orders.ToList();
    }

    public decimal GetTotal()
    {
        return Orders
            .Where(o => o.Status != OrderStatus.Canceled)
            .Sum(o => o.GetTotal());
    }

    // ========== Tab Status Management Methods ==========

    public void WaitForPayment()
    {
        if (Status != TabStatus.Open)
            throw new InvalidOperationException("Only open tabs can be set to wait for payment.");

        Status = TabStatus.WaitingForPayment;
    }

    public void Close()
    {
        if (Status != TabStatus.WaitingForPayment)
            throw new InvalidOperationException("Only tabs waiting for payment can be closed.");

        bool hasOpenedOrders = Orders.Any(order => order.IsOpen);

        if(hasOpenedOrders)
            throw new InvalidOperationException("Cannot close tab with opened orders.");

        Status = TabStatus.Closed;
        ClosedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != TabStatus.Open)
            throw new InvalidOperationException("Cannot cancel a tab that is not open.");

        Status = TabStatus.Canceled;
        ClosedAt = DateTime.UtcNow;
    }

    // ======================================================

    private void ValidateTabNumber(int tabNumber)
    {
        if (tabNumber <= 0)
            throw new ArgumentException("Tab number must be greater than zero.", nameof(tabNumber));
    }

    private void ValidateServiceType(ServiceType serviceType)
    {
        if (!Enum.IsDefined(typeof(ServiceType), serviceType))
            throw new ArgumentException("Invalid service type.", nameof(serviceType));
    }
}
