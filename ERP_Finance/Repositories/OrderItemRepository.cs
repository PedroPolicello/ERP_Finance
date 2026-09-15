using ERP_Finance.Data;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        ArgumentNullException.ThrowIfNull(orderItem);

        _context.OrderItems.Add(orderItem);
        var result = _context.SaveChanges();

        return result > 0;
    }

    public OrderItem? GetOrderItemById(Guid id) => _context.OrderItems.Include(orderItem => orderItem.Product)
                                                                      .FirstOrDefault(orderItem => orderItem.Id == id);

}
