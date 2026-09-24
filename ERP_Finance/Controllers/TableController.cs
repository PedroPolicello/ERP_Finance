using ERP_Finance.DTOs.Table;
using ERP_Finance.Entities;
using ERP_Finance.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERP_Finance.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TableController : ControllerBase
{
    private readonly TableService _tableService;

    public TableController(TableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet("{id:guid}")]
    public ActionResult<Table> GetTable(Guid id)
    {
        var table = _tableService.GetTableByIdService(id);

        return Ok(table);
    }

    [HttpGet("{tableNumber:int}")]
    public ActionResult<Table> GetTableByNumber(int tableNumber)
    {
        var table = _tableService.GetTableByNumberService(tableNumber);

        return Ok(table);
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<Table>> GetAllTables()
    {
        var tables = _tableService.GetAllTablesService();

        return Ok(tables);
    }

    [HttpGet("{tableId:guid}/open-tabs")]
    public ActionResult<IReadOnlyList<Tab>> GetOpenTabsByTableId(Guid tableId)
    {
        var tabs = _tableService.GetOpenTabsByTableIdService(tableId);

        return Ok(tabs);
    }

    [HttpPost]
    public ActionResult CreateTable([FromBody] CreateTableDTO tableDTO)
    {
        var result = _tableService.CreateTableService(tableDTO);

        return CreatedAtAction(nameof(GetTable), new { id = result.Id }, result);
    }



}
