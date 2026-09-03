using ERP_Finance.DTOs.OrderItem;
using ERP_Finance.Entities;
using ERP_Finance.Services;
using ERP_Finance.Tests.Fakes;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Unit.Services;

public class OrderItemServiceTests
{
    [Fact]
    public void CreateOrderItemService_WithValidDtoAndExistingProduct_ShouldCreateAndSaveOrderItem()
    {
        // Arrange
        var fakeProductRepository = new FakeProductRepository();
        var fakeOrderItemRepository = new FakeOrderItemRepository();

        var service = new OrderItemService(
            fakeOrderItemRepository,
            fakeProductRepository);

        var product = new Product(
            sku: "123-456-789",
            name: "Test product",
            description: "Product used for OrderItemService unit test.",
            price: 5.69m,
            category: ProductCategory.Salgados,
            details: new ProductDetails(
                brandName: "Test brand",
                weightOrVolume: 1m,
                measureType: MeasureType.Unit),
            createdAt: DateTime.UtcNow);

        fakeProductRepository.AddToRepository(product);

        var dto = new CreateOrderItemDTO
        {
            OrderId = Guid.NewGuid(),
            ProductId = product.Id,
            Quantity = 2.5m
        };

        decimal expectedSubtotal = dto.Quantity * product.Price;

        // Act
        var result = service.CreateOrderItemService(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);

        Assert.Equal(dto.OrderId, result.OrderId);
        Assert.Equal(dto.ProductId, result.ProductId);
        Assert.Equal(dto.Quantity, result.Quantity);

        Assert.Equal(product.Price, result.UnitPrice);
        Assert.Equal(expectedSubtotal, result.Subtotal);

        Assert.Single(fakeOrderItemRepository.OrderItems);
        Assert.Same(result, fakeOrderItemRepository.OrderItems.Single());
    }

    [Fact]
    public void CreateOrderItemService_WithNonExistingProduct_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var fakeProductRepository = new FakeProductRepository();
        var fakeOrderItemRepository = new FakeOrderItemRepository();

        var service = new OrderItemService(
            fakeOrderItemRepository,
            fakeProductRepository);

        var dto = new CreateOrderItemDTO
        {
            OrderId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 2.5m
        };

        // Act
        var exception = Assert.Throws<KeyNotFoundException>(
            () => service.CreateOrderItemService(dto));

        // Assert
        Assert.Equal("Product not found.", exception.Message);
        Assert.Empty(fakeOrderItemRepository.OrderItems);
    }

    [Fact]
    public void CreateOrderItemService_WithNullDto_ShouldThrowArgumentNullException()
    {
        // Arrange
        var fakeProductRepository = new FakeProductRepository();
        var fakeOrderItemRepository = new FakeOrderItemRepository();

        var service = new OrderItemService(
            fakeOrderItemRepository,
            fakeProductRepository);

        CreateOrderItemDTO? dto = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(
            () => service.CreateOrderItemService(dto!));

        // Assert
        Assert.Equal("orderItemDTO", exception.ParamName);
        Assert.Empty(fakeOrderItemRepository.OrderItems);
    }

    [Fact]
    public void CreateOrderItemService_WhenRepositoryFailsToSave_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var fakeProductRepository = new FakeProductRepository();

        var fakeOrderItemRepository = new FakeOrderItemRepository
        {
            ShouldFailOnAdd = true
        };

        var service = new OrderItemService(
            fakeOrderItemRepository,
            fakeProductRepository);

        var product = new Product(
            sku: "123-456-789",
            name: "Test product",
            description: "Product used for OrderItemService unit test.",
            price: 5.69m,
            category: ProductCategory.Salgados,
            details: new ProductDetails(
                brandName: "Test brand",
                weightOrVolume: 1m,
                measureType: MeasureType.Unit),
            createdAt: DateTime.UtcNow);

        fakeProductRepository.AddToRepository(product);

        var dto = new CreateOrderItemDTO
        {
            OrderId = Guid.NewGuid(),
            ProductId = product.Id,
            Quantity = 2.5m
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => service.CreateOrderItemService(dto));

        // Assert
        Assert.Equal(
            "The order item could not be created.",
            exception.Message);

        Assert.Empty(fakeOrderItemRepository.OrderItems);
    }
}