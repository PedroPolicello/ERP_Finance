using ERP_Finance.Data;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;
using ERP_Finance.Types;
using Microsoft.EntityFrameworkCore;

namespace ERP_Finance.Repositories;

public class TableRepository : ITableRepository
{
    private readonly AppDbContext _context;

    public TableRepository(AppDbContext context)
    {
        _context = context;
    }

    public bool AddToRepository(Table table)
    {
        ArgumentNullException.ThrowIfNull(table);

        _context.Tables.Add(table);
        var result = _context.SaveChanges();

        return result > 0;
    }

    public Table? GetTableById(Guid id) => _context.Tables.Include(table => table.Tabs)
                                                          .ThenInclude(tab => tab.Orders)
                                                          .ThenInclude(order => order.OrderItems)
                                                          .ThenInclude(orderItem => orderItem.Product)
                                                          .FirstOrDefault(t => t.Id == id);

    public Table? GetTableByNumber(int tableNumber) => _context.Tables.Include(table => table.Tabs)
                                                                      .ThenInclude(tab => tab.Orders)
                                                                      .ThenInclude(order => order.OrderItems)
                                                                      .ThenInclude(orderItem => orderItem.Product)
                                                                      .FirstOrDefault(t => t.TableNumber == tableNumber);

    public IReadOnlyList<Table> GetAllTables() => _context.Tables.Include(table => table.Tabs)
                                                                 .ThenInclude(tab => tab.Orders)
                                                                 .ThenInclude(order => order.OrderItems)
                                                                 .ThenInclude(orderItem => orderItem.Product)
                                                                 .ToList();

    public IReadOnlyList<Tab> GetOpenTabsByTableId(Guid tableId) => _context.Tabs.Include(tab => tab.Orders)
                                                                                 .ThenInclude(order => order.OrderItems)
                                                                                 .ThenInclude(orderItem => orderItem.Product)
                                                                                 .Where(tab => tab.TableId == tableId && tab.Status == TabStatus.Open)
                                                                                 .ToList();
}
