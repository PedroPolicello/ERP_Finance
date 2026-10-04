using ERP_Finance.DTOs.Order;
using ERP_Finance.Entities;
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
    public ActionResult<Order> GetOrder(Guid id)
    {
        var order = _orderService.GetOrderByIdService(id);

        return Ok(order);
    }

    [HttpPost]
    public ActionResult CreateOrder([FromBody] CreateOrderDTO orderDTO)
    {
        var result = _orderService.CreateOrderService(orderDTO);

        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }

    // ================ Order Status Endpoints ================
    [HttpPatch("{orderId:guid}/start-preparing")]
    public ActionResult StartPreparingOrder(Guid orderId)
    {
        var order = _orderService.StartPreparingOrderService(orderId);

        return Ok(order);
    }

    [HttpPatch("{orderId:guid}/ready-to-deliver")]
    public ActionResult ReadyToDeliverOrder(Guid orderId)
    {
        var order = _orderService.ReadyToDeliverOrderService(orderId);

        return Ok(order);
    }

    [HttpPatch("{orderId:guid}/delivered")]
    public ActionResult DeliveredOrder(Guid orderId)
    {
        var order = _orderService.DeliveredOrderService(orderId);

        return Ok(order);
    }

    [HttpPatch("{orderId:guid}/cancel")]
    public ActionResult CancelOrder(Guid orderId)
    {
        var order = _orderService.CancelOrderService(orderId);

        return Ok(order);
    }

    // ========================================================

}
