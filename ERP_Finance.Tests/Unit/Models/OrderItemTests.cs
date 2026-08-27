using ERP_Finance.Entities;

namespace ERP_Finance.Tests.Unit.Models;

public class OrderItemTests
{
    [Fact]
    public void CreateOrderItem_WithValidData_ShouldCreateOrderItem()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        decimal quantity = 2.5m;
        decimal unitPrice = 5.69m;

        // Act
        var orderItem = new OrderItem(orderId, productId, quantity, unitPrice);

        // Assert
        Assert.NotEqual(Guid.Empty, orderItem.Id);
        Assert.Equal(orderId, orderItem.OrderId);
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(quantity, orderItem.Quantity);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
    }

    [Fact]
    public void CreateOrderItem_WithValidData_ShouldCalculateCorrectSubtotal()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        decimal quantity = 2.5m;
        decimal unitPrice = 5.69m;
        decimal expectedSubtotal = quantity * unitPrice;

        // Act
        var orderItem = new OrderItem(orderId, productId, quantity, unitPrice);

        // Assert
        Assert.Equal(expectedSubtotal, orderItem.Subtotal);
    }

    [Fact]
    public void UpdateQuantity_WithValidData_ShouldUpdateQuantityAndRecalculateSubtotal()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        decimal initialQuantity = 2.5m;
        decimal updatedQuantity = 9.2m;
        decimal unitPrice = 5.69m;
        decimal expectedInitialSubtotal = initialQuantity * unitPrice;
        decimal expectedUpdatedSubtotal = updatedQuantity * unitPrice;

        var orderItem = new OrderItem(
            orderId,
            productId,
            initialQuantity,
            unitPrice);

        // Assert
        Assert.Equal(expectedInitialSubtotal, orderItem.Subtotal);

        // Act
        orderItem.UpdateQuantity(updatedQuantity);

        // Assert
        Assert.Equal(updatedQuantity, orderItem.Quantity);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
        Assert.Equal(expectedUpdatedSubtotal, orderItem.Subtotal);
    }

    [Fact]
    public void CreateOrderItem_WithEmptyOrderId_ShouldThrowArgumentException()
    {
        // Arrange
        Guid orderId = Guid.Empty;
        Guid productId = Guid.NewGuid();
        decimal quantity = 2.5m;
        decimal unitPrice = 5.69m;

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => new OrderItem(orderId, productId, quantity, unitPrice));

        // Assert
        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void CreateOrderItem_WithEmptyProductId_ShouldThrowArgumentException()
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.Empty;
        decimal quantity = 2.5m;
        decimal unitPrice = 5.69m;

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => new OrderItem(orderId, productId, quantity, unitPrice));

        // Assert
        Assert.Equal("id", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5.2)]
    public void CreateOrderItem_WithZeroOrNegativeQuantity_ShouldThrowArgumentOutOfRangeException(decimal value)
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        decimal unitPrice = 5.69m;

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new OrderItem(orderId, productId, value, unitPrice));

        // Assert
        Assert.Equal("quantity", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10.69)]
    public void CreateOrderItem_WithZeroOrNegativeUnitPrice_ShouldThrowArgumentOutOfRangeException(decimal value)
    {
        // Arrange
        Guid orderId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        decimal quantity = 2.5m;

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => new OrderItem(orderId, productId, quantity, value));

        // Assert
        Assert.Equal("unitPrice", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5.2)]
    public void UpdateQuantity_WithZeroOrNegativeQuantity_ShouldThrowArgumentOutOfRangeException(decimal value)
    {
        // Arrange
        var orderItem = new OrderItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            2.5m,
            5.69m);

        // Act
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => orderItem.UpdateQuantity(value));

        // Assert
        Assert.Equal("quantity", exception.ParamName);
    }
}