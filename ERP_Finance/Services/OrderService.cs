using ERP_Finance.DTOs.Order;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }


    public Order CreateOrderService(CreateOrderDTO orderDTO)
    {
        if (orderDTO == null)
            throw new ArgumentNullException(nameof(orderDTO));

        var orderNumber = GetNextOrderNumber(orderDTO.TabId);

        var order = new Order(
            orderNumber: orderNumber,
            tabId: orderDTO.TabId,
            note: orderDTO.Note
        );

        var created = _orderRepository.AddToRepository(order);

        if (!created)
            throw new InvalidOperationException("The order could not be created");

        return order;
    }

    public Order StartPreparingOrderService(Guid orderId)
    {
        var order = _orderRepository.GetOrderById(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found");

        order.StartPreparing();

        var updated = _orderRepository.UpdateInRepository();

        if (!updated)
            throw new InvalidOperationException("The order status could not be updated");

        return order;
    }

    public Order ReadyToDeliverOrderService(Guid orderId)
    {
        var order = _orderRepository.GetOrderById(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found");

        order.ReadyToDeliver();

        var updated = _orderRepository.UpdateInRepository();

        if (!updated)
            throw new InvalidOperationException("The order status could not be updated");

        return order;
    }

    public Order DeliveredOrderService(Guid orderId)
    {
        var order = _orderRepository.GetOrderById(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found");

        order.Delivered();

        var updated = _orderRepository.UpdateInRepository();

        if (!updated)
            throw new InvalidOperationException("The order status could not be updated");

        return order;
    }

    public Order CancelOrderService(Guid orderId)
    {
        var order = _orderRepository.GetOrderById(orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found");

        order.Cancel();

        var updated = _orderRepository.UpdateInRepository();

        if (!updated)
            throw new InvalidOperationException("The order status could not be updated");

        return order;
    }

    public Order GetOrderByIdService(Guid id)
    {
        var order = _orderRepository.GetOrderById(id);

        if (order == null)
            throw new KeyNotFoundException("Order not found");

        return order;
    }

    public IReadOnlyList<Order> GetAllOrdersService()
    {
        return _orderRepository.GetAllOrders();
    }

    private int GetNextOrderNumber(Guid tabId)
    {
        var lastOrderNumber = _orderRepository.GetLastOrderNumberFromTab(tabId);

        return lastOrderNumber + 1;
    }

}
