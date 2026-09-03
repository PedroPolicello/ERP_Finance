using ERP_Finance.Data;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly AppDbContext _context;

    public OrderItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public bool AddToRepository(OrderItem orderItem)
    {
        if (orderItem == null)
            return false;

        _context.OrderItems.Add(orderItem);
        _context.SaveChanges();

        return true;
    }

    public OrderItem? GetOrderItemById(Guid id) => _context.OrderItems.FirstOrDefault(orderItem => orderItem.Id == id);

}
