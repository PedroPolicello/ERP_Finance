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
            unitPrice: product.Price
        );

        var created = _orderItemRepository.AddToRepository(orderItem);

        if (!created)
            throw new InvalidOperationException("The order item could not be created.");

        return orderItem;
    }

    public OrderItem GetOrderItemService(Guid id)
    {
        var orderItem = _orderItemRepository.GetOrderItemById(id);

        if (orderItem == null)
            throw new KeyNotFoundException("Order item not found.");

        return orderItem;
    }
}
