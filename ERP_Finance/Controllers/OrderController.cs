using ERP_Finance.DTOs.Order;
using ERP_Finance.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERP_Finance.Controllers;


[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id:guid}")]
    public ActionResult GetOrder(Guid id)
    {
        var order = _orderService.GetOrderService(id);

        return Ok(order);
    }

    [HttpPost]
    public ActionResult CreateOrder([FromBody] CreateOrderDTO orderDTO)
    {
        var result = _orderService.CreateOrderService(orderDTO);

        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }
}
