using ERP_Finance.Entities;

namespace ERP_Finance.Repositories.Interfaces;

public interface IOrderItemRepository
{
    bool AddToRepository(OrderItem orderItem);
    public bool UpdateInRepository();
    public bool RemoveFromRepository(OrderItem orderItem);
    OrderItem? GetOrderItemById(Guid id);
    IReadOnlyList<OrderItem> GetAllOrderItems();
}
