using ERP_Finance.DTOs.OrderItem;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;
using ERP_Finance.Types;

namespace ERP_Finance.Services;

public class OrderItemService
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderItemService(IOrderItemRepository orderItemRepository, IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public OrderItem CreateOrderItemService(CreateOrderItemDTO orderItemDTO)
    {
        if (orderItemDTO == null)
            throw new ArgumentNullException(nameof(orderItemDTO));

        var product = _productRepository.GetProductById(orderItemDTO.ProductId);

        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        var orderItem = new OrderItem(
            orderId: orderItemDTO.OrderId,
            productId: orderItemDTO.ProductId,
            quantity: orderItemDTO.Quantity,
            unitPrice: product.Price,
            note: orderItemDTO.Note
        );

        var created = _orderItemRepository.AddToRepository(orderItem);

        if (!created)
            throw new InvalidOperationException("The order item could not be created.");

        return orderItem;
    }

    public bool UpdateOrderItemService(Guid orderItemId, UpdateOrderItemDTO orderItemDTO)
    {
        if (orderItemDTO == null)
            throw new ArgumentNullException(nameof(orderItemDTO));

        var existingOrderItem = _orderItemRepository.GetOrderItemById(orderItemId);
        if (existingOrderItem == null)
            throw new KeyNotFoundException("Order item not found.");

        EnsureOrderIsEditable(existingOrderItem.OrderId);

        if (orderItemDTO.Quantity.HasValue && orderItemDTO.Quantity.Value != existingOrderItem.Quantity)
            existingOrderItem.UpdateQuantity(orderItemDTO.Quantity.Value);

        if (orderItemDTO.UpdateNoteField)
            existingOrderItem.UpdateNote(orderItemDTO.Note);

        _orderItemRepository.UpdateInRepository();

        return true;
    }

    public bool DeleteOrderItemService(Guid orderItemId)
    {
        var existingOrderItem = _orderItemRepository.GetOrderItemById(orderItemId);
        if (existingOrderItem == null)
            throw new KeyNotFoundException("Order item not found.");

        EnsureOrderIsEditable(existingOrderItem.OrderId);

        var deleted = _orderItemRepository.RemoveFromRepository(existingOrderItem);
        if (!deleted)
            throw new InvalidOperationException("The order item could not be deleted.");

        return true;
    }

    public OrderItem GetOrderItemByIdService(Guid id)
    {
        var orderItem = _orderItemRepository.GetOrderItemById(id);

        if (orderItem == null)
            throw new KeyNotFoundException("Order item not found.");

        return orderItem;
    }

    public IReadOnlyList<OrderItem> GetAllOrderItemsService()
    {
        return _orderItemRepository.GetAllOrderItems();
    }

    private void EnsureOrderIsEditable(Guid orderId)
    {
        var order = _orderRepository.GetOrderById(orderId);
        if (order == null)
            throw new KeyNotFoundException("Order not found.");

        if (order.Status == OrderStatus.ReadyToDeliver || order.Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Cannot edit or remove an item from an order that is ready to deliver or already delivered.");
    }
}
