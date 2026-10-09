using ERP_Finance.DTOs.OrderItem;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Services;

public class OrderItemService
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;

    public OrderItemService(IOrderItemRepository orderItemRepository, IProductRepository productRepository)
    {
        _orderItemRepository = orderItemRepository;
        _productRepository = productRepository;
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

        if (orderItemDTO.Quantity != existingOrderItem.Quantity)
            existingOrderItem.UpdateQuantity(orderItemDTO.Quantity);

        if (orderItemDTO.Note != existingOrderItem.Note)
            existingOrderItem.UpdateNote(orderItemDTO.Note);

        var updated = _orderItemRepository.UpdateInRepository();
        if (!updated)
            throw new InvalidOperationException("The order item could not be updated.");

        return true;
    }

    public bool DeleteOrderItemService(Guid orderItemId)
    {
        var existingOrderItem = _orderItemRepository.GetOrderItemById(orderItemId);
        if (existingOrderItem == null)
            throw new KeyNotFoundException("Order item not found.");

        _orderItemRepository.RemoveFromRepository(existingOrderItem);

        var deleted = _orderItemRepository.UpdateInRepository();
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
}
