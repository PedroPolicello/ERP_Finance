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

    // ================ Tab Status Endpoints ================

    [HttpPatch("{tabId:guid}/wait-for-payment")]
    public ActionResult TabToWaitingForPayment(Guid tabId)
    {
        var tab = _tabService.TabToWaitingForPaymentService(tabId);

        return Ok(tab);
    }

    [HttpPatch("{tabId:guid}/cancel")]
    public ActionResult CancelTab(Guid tabId, [FromBody] CancelTabDTO cancelTabDTO)
    {
        var tab = _tabService.CancelTabService(tabId, cancelTabDTO);
        return Ok(tab);
    }

    // TODO: TEMPORÁRIO — este endpoint existe só para testar o fluxo de Comanda
    // antes do módulo de Pagamentos existir. Quando Pagamentos for implementado,
    // REMOVER este endpoint (ou travar atrás de uma verificação de pagamento confirmado) —
    // hoje qualquer chamada fecha a comanda sem nenhuma validação de pagamento.
    [HttpPatch("{tabId:guid}/close")]
    public ActionResult CloseTab(Guid tabId)
    {
        var tab = _tabService.CloseTabService(tabId);

        return Ok(tab);
    }

    // ======================================================


}
