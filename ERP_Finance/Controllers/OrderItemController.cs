using ERP_Finance.DTOs.OrderItem;
using ERP_Finance.Entities;
using ERP_Finance.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERP_Finance.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderItemController : ControllerBase
{
    private readonly OrderItemService _orderItemService;

    public OrderItemController(OrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    [HttpGet("{id:guid}")]
    public ActionResult<OrderItem> GetOrderItem(Guid id)
    {
        var orderItem = _orderItemService.GetOrderItemService(id);

        return Ok(orderItem);
    }

    [HttpPost]
    public ActionResult CreateOrderItem([FromBody] CreateOrderItemDTO orderItemDTO)
    {
        var result = _orderItemService.CreateOrderItemService(orderItemDTO);

        return CreatedAtAction(nameof(GetOrderItem), new { id = result.Id }, result);
    }
}
