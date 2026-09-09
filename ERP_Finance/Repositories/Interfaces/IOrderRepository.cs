using ERP_Finance.Entities;

namespace ERP_Finance.Repositories.Interfaces;

public interface IOrderRepository
{
    bool AddToRepository(Order order);
    Order? GetOrderById(Guid id);
}
