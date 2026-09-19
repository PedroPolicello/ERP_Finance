using ERP_Finance.DTOs.Tab;
using ERP_Finance.Entities;
using ERP_Finance.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERP_Finance.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TabController: ControllerBase
{
    private readonly TabService _tabService;

    public TabController(TabService tabService)
    {
        _tabService = tabService;
    }

    [HttpGet("{id:guid}")]
    public ActionResult<Tab> GetTab(Guid id)
    {
        var tab = _tabService.GetTabByIdService(id);

        return Ok(tab);
    }


    [HttpGet("{tabNumber:int}")]
    public ActionResult<Tab> GetTabByNumber(int tabNumber)
    {
        var tab = _tabService.GetTabByNumberService(tabNumber);

        return Ok(tab);
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<Tab>> GetAllTabs()
    {
        var tabs = _tabService.GetAllTabsService();

        return Ok(tabs);
    }

    [HttpPost]
    public ActionResult CreateTab([FromBody] CreateTabDTO tabDTO)
    {
        var result = _tabService.CreateTabService(tabDTO);

        return CreatedAtAction(nameof(GetTab), new { id = result.Id }, result);
    }

}
