using ERP_Finance.Data;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERP_Finance.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public bool AddToRepository(Order order)
    {
        if (order == null)
            return false;

        _context.Orders.Add(order);
        var result = _context.SaveChanges();

        return result > 0;
    }

    public Order? GetOrderById(Guid id) => _context.Orders
                                                    .Include(order => order.OrderItems)
                                                    .ThenInclude(orderItem => orderItem.Product)
                                                    .FirstOrDefault(order => order.Id == id);
}
