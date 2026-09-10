using ERP_Finance.Entities;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Unit.Models;

public class OrderTests
{
    [Fact]
    public void CreateOrder_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        int orderNumber = 1;
        Guid tabId = Guid.NewGuid();
        string note = "Customer requested no napkins.";

        // Act
        var order = new Order(
            orderNumber: orderNumber,
            tabId: tabId,
            note: note);

        // Assert
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(orderNumber, order.OrderNumber);
        Assert.Equal(tabId, order.TabId);
        Assert.Equal(OrderStatus.SentToKitchen, order.Status);
        Assert.Equal(note, order.Note);
        Assert.NotEqual(default, order.CreatedAt);
        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void CreateOrder_WithoutStatus_ShouldUseSentToKitchenAsDefaultStatus()
    {
        // Arrange
        int orderNumber = 1;
        Guid tabId = Guid.NewGuid();

        // Act
        var order = new Order(
            orderNumber: orderNumber,
            tabId: tabId);

        // Assert
        Assert.Equal(OrderStatus.SentToKitchen, order.Status);
    }

    [Fact]
    public void CreateOrder_WithInformedStatus_ShouldUseInformedStatus()
    {
        // Arrange
        int orderNumber = 1;
        Guid tabId = Guid.NewGuid();
        OrderStatus status = OrderStatus.Preparing;

        // Act
        var order = new Order(
            orderNumber: orderNumber,
            tabId: tabId,
            status: status);

        // Assert
        Assert.Equal(status, order.Status);
    }

    [Fact]
    public void CreateOrder_WithoutNote_ShouldCreateOrderWithNullNote()
    {
        // Arrange
        int orderNumber = 1;
        Guid tabId = Guid.NewGuid();

        // Act
        var order = new Order(
            orderNumber: orderNumber,
            tabId: tabId);

        // Assert
        Assert.Null(order.Note);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateOrder_WithZeroOrNegativeOrderNumber_ShouldThrowArgumentException(
        int invalidOrderNumber)
    {
        // Arrange
        Guid tabId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => new Order(
                orderNumber: invalidOrderNumber,
                tabId: tabId));

        // Assert
        Assert.Equal(
            "Order number must be a positive integer.",
            exception.Message);
    }

    [Fact]
    public void CreateOrder_WithEmptyTabId_ShouldThrowArgumentException()
    {
        // Arrange
        int orderNumber = 1;
        Guid tabId = Guid.Empty;

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => new Order(
                orderNumber: orderNumber,
                tabId: tabId));

        // Assert
        Assert.Equal(
            "Tab ID must be a valid GUID.",
            exception.Message);
    }

    [Fact]
    public void AddOrderItem_WithOrderItemFromSameOrder_ShouldAddOrderItem()
    {
        // Arrange
        var order = new Order(
            orderNumber: 1,
            tabId: Guid.NewGuid());

        var orderItem = new OrderItem(
            orderId: order.Id,
            productId: Guid.NewGuid(),
            quantity: 2.5m,
            unitPrice: 5.69m);

        // Act
        order.AddOrderItem(orderItem);

        // Assert
        Assert.Single(order.OrderItems);
        Assert.Contains(orderItem, order.OrderItems);
    }

    [Fact]
    public void AddOrderItem_WithNullOrderItem_ShouldThrowArgumentNullException()
    {
        // Arrange
        var order = new Order(
            orderNumber: 1,
            tabId: Guid.NewGuid());

        OrderItem? orderItem = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(
            () => order.AddOrderItem(orderItem!));

        // Assert
        Assert.Equal("item", exception.ParamName);
        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void AddOrderItem_WithOrderItemFromDifferentOrder_ShouldThrowArgumentException()
    {
        // Arrange
        var order = new Order(
            orderNumber: 1,
            tabId: Guid.NewGuid());

        var orderItemFromAnotherOrder = new OrderItem(
            orderId: Guid.NewGuid(),
            productId: Guid.NewGuid(),
            quantity: 2.5m,
            unitPrice: 5.69m);

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => order.AddOrderItem(orderItemFromAnotherOrder));

        // Assert
        Assert.Equal(
            "Order item does not belong to this order.",
            exception.Message);

        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void RemoveOrderItem_WithExistingOrderItem_ShouldRemoveOrderItem()
    {
        // Arrange
        var order = new Order(
            orderNumber: 1,
            tabId: Guid.NewGuid());

        var orderItem = new OrderItem(
            orderId: order.Id,
            productId: Guid.NewGuid(),
            quantity: 2.5m,
            unitPrice: 5.69m);

        order.AddOrderItem(orderItem);

        // Act
        order.RemoveOrderItem(orderItem);

        // Assert
        Assert.Empty(order.OrderItems);
        Assert.DoesNotContain(orderItem, order.OrderItems);
    }

    [Fact]
    public void RemoveOrderItem_WithNullOrderItem_ShouldThrowArgumentNullException()
    {
        // Arrange
        var order = new Order(
            orderNumber: 1,
            tabId: Guid.NewGuid());

        OrderItem? orderItem = null;

        // Act
        var exception = Assert.Throws<ArgumentNullException>(
            () => order.RemoveOrderItem(orderItem!));

        // Assert
        Assert.Equal("item", exception.ParamName);
    }

    [Fact]
    public void GetOrderItem_WithExistingOrderItemId_ShouldReturnOrderItem()
    {
        // Arrange
        var order = new Order(
            orderNumber: 1,
            tabId: Guid.NewGuid());

        var firstOrderItem = new OrderItem(
            orderId: order.Id,
            productId: Guid.NewGuid(),
            quantity: 2.5m,
            unitPrice: 5.69m);

        var secondOrderItem = new OrderItem(
            orderId: order.Id,
            productId: Guid.NewGuid(),
            quantity: 1m,
            unitPrice: 10m);

        order.AddOrderItem(firstOrderItem);
        order.AddOrderItem(secondOrderItem);

        // Act
        var result = order.GetOrderItem(secondOrderItem.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Same(secondOrderItem, result);
    }

    [Fact]
    public void GetOrderItem_WithNonExistingOrderItemId_ShouldReturnNull()
    {
        // Arrange
        var order = new Order(
            orderNumber: 1,
            tabId: Guid.NewGuid());

        var orderItem = new OrderItem(
            orderId: order.Id,
            productId: Guid.NewGuid(),
            quantity: 2.5m,
            unitPrice: 5.69m);

        order.AddOrderItem(orderItem);

        // Act
        var result = order.GetOrderItem(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
}