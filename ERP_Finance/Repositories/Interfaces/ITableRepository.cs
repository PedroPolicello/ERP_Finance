using ERP_Finance.Entities;

namespace ERP_Finance.Repositories.Interfaces;

public interface ITableRepository
{
    bool AddToRepository(Table table);
    Table? GetTableById(Guid id);
    Table? GetTableByNumber(int tableNumber);
    IReadOnlyList<Table> GetAllTables();
    IReadOnlyList<Tab> GetOpenTabsByTableId(Guid id);
}
