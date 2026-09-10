using ERP_Finance.DTOs.Order;
using ERP_Finance.Entities;
using ERP_Finance.Services;
using ERP_Finance.Tests.Fakes;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Unit.Services;

public class OrderServiceTests
{
    [Fact]
    public void CreateOrderService_WithValidDto_ShouldCreateAndSaveOrder()
    {
        // Arrange
        var fakeOrderRepository = new FakeOrderRepository();

        var service = new OrderService(fakeOrderRepository);

        var dto = new CreateOrderDTO
        {
            OrderNumber = 1,
            TabId = Guid.NewGuid(),
            Note = "Customer requested no napkins."
        };

        // Act
        var result = service.CreateOrderService(dto);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(dto.OrderNumber, result.OrderNumber);
        Assert.Equal(dto.TabId, result.TabId);
        Assert.Equal(dto.Note, result.Note);
        Assert.Equal(OrderStatus.SentToKitchen, result.Status);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.Empty(result.OrderItems);

        Assert.Single(fakeOrderRepository.Orders);
        Assert.Same(result, fakeOrderRepository.Orders.Single());
    }

    [Fact]
    public void CreateOrderService_WithNullDto_ShouldThrowArgumentNullException()
    {
        // Arrange
        var fakeOrderRepository = new FakeOrderRepository();

        var service = new OrderService(fakeOrderRepository);

        CreateOrderDTO? dto = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(
            () => service.CreateOrderService(dto!));

        // Assert
        Assert.Equal("orderDTO", exception.ParamName);
        Assert.Empty(fakeOrderRepository.Orders);
    }

    [Fact]
    public void CreateOrderService_WhenRepositoryFailsToSave_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var fakeOrderRepository = new FakeOrderRepository
        {
            ShouldFailOnAdd = true
        };

        var service = new OrderService(fakeOrderRepository);

        var dto = new CreateOrderDTO
        {
            OrderNumber = 1,
            TabId = Guid.NewGuid(),
            Note = "Test note."
        };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => service.CreateOrderService(dto));

        // Assert
        Assert.Equal("The order could not be created", exception.Message);
        Assert.Empty(fakeOrderRepository.Orders);
    }

    [Fact]
    public void GetOrderService_WithExistingOrderId_ShouldReturnOrder()
    {
        // Arrange
        var fakeOrderRepository = new FakeOrderRepository();

        var service = new OrderService(fakeOrderRepository);

        var order = new Order(
            orderNumber: 1,
            tabId: Guid.NewGuid(),
            note: "Existing order.");

        fakeOrderRepository.AddToRepository(order);

        // Act
        var result = service.GetOrderService(order.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Same(order, result);
        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.OrderNumber, result.OrderNumber);
        Assert.Equal(order.TabId, result.TabId);
        Assert.Equal(order.Status, result.Status);
        Assert.Equal(order.Note, result.Note);
    }

    [Fact]
    public void GetOrderService_WithNonExistingOrderId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var fakeOrderRepository = new FakeOrderRepository();

        var service = new OrderService(fakeOrderRepository);

        Guid nonExistingOrderId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<KeyNotFoundException>(
            () => service.GetOrderService(nonExistingOrderId));

        // Assert
        Assert.Equal("Order not found", exception.Message);
    }
}