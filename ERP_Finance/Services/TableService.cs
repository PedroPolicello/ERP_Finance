using ERP_Finance.DTOs.Table;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Services;

public class TableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public Table CreateTableService(CreateTableDTO tableDTO)
    {
        if(tableDTO == null)
            throw new ArgumentNullException(nameof(tableDTO));

        var table = new Table(tableNumber: tableDTO.TableNumber);

        var created = _tableRepository.AddToRepository(table);

        if(!created)
            throw new KeyNotFoundException("Table could not be created.");

        return table;
    }

    public Table? GetTableByIdService(Guid guid)
    {
        var table = _tableRepository.GetTableById(guid);

        if(table == null)
            throw new KeyNotFoundException("Table not found.");

        return table;
    }

    public Table? GetTableByNumberService(int tableNumber)
    {
        var table = _tableRepository.GetTableByNumber(tableNumber);

        if (table == null)
            throw new KeyNotFoundException("Table not found.");

        return table;
    }

    public IReadOnlyList<Table> GetAllTablesService()
    {
        return _tableRepository.GetAllTables();
    }

    public IReadOnlyList<Tab> GetOpenTabsByTableIdService(Guid tableId)
    {
        return _tableRepository.GetOpenTabsByTableId(tableId);
    }
}
