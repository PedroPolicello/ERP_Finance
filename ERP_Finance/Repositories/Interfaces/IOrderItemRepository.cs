using ERP_Finance.Entities;

namespace ERP_Finance.Repositories.Interfaces;

public interface IOrderItemRepository
{
    bool AddToRepository(OrderItem orderItem);
    OrderItem? GetOrderItemById(Guid id);

}
