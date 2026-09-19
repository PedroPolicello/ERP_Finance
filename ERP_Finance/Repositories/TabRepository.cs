using ERP_Finance.Data;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERP_Finance.Repositories;

public class TabRepository : ITabRepository
{
    private readonly AppDbContext _context;

    public TabRepository(AppDbContext context)
    {
        _context = context;
    }

    public bool AddToRepository(Tab tab)
    {
        ArgumentNullException.ThrowIfNull(tab);

        _context.Tabs.Add(tab);
        var result = _context.SaveChanges();

        return result > 0;
    }

    public Tab? GetTabById(Guid id) => _context.Tabs.Include(tab => tab.Orders)
                                                    .ThenInclude(order => order.OrderItems)
                                                    .ThenInclude(orderItem => orderItem.Product)
                                                    .FirstOrDefault(tab => tab.Id == id);

    public Tab? GetTabByNumber(int tabNumber) => _context.Tabs.Include(tab => tab.Orders)
                                                              .ThenInclude(order => order.OrderItems)
                                                              .ThenInclude(orderItem => orderItem.Product)
                                                              .FirstOrDefault(tab => tab.TabNumber == tabNumber);


    // Ajustar metodo para melhorar performance. (Futuro)
    public IReadOnlyList<Tab> GetAllTabs() => _context.Tabs.Include(tab => tab.Orders)
                                                           .ThenInclude(order => order.OrderItems)
                                                           .ThenInclude(orderItem => orderItem.Product)
                                                           .ToList();


}
