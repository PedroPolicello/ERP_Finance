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
        var orderItem = _orderItemService.GetOrderItemByIdService(id);

        return Ok(orderItem);
    }

    [HttpPost]
    public ActionResult CreateOrderItem([FromBody] CreateOrderItemDTO orderItemDTO)
    {
        var result = _orderItemService.CreateOrderItemService(orderItemDTO);

        return CreatedAtAction(nameof(GetOrderItem), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}")]
    public ActionResult UpdateOrderItem(Guid id, [FromBody] UpdateOrderItemDTO orderItemDTO)
    {
        var updated = _orderItemService.UpdateOrderItemService(id, orderItemDTO);

        if (!updated)
            return BadRequest();

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public ActionResult DeleteOrderItem(Guid id)
    {
        _orderItemService.DeleteOrderItemService(id);

        return NoContent();
    }
}
