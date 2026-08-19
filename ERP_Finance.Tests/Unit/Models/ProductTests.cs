using ERP_Finance.Models;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Unit.Models;

public class ProductTests
{
    [Fact]
    public void CreateProduct_WithValidData_ShouldCreateProduct()
    {
        // Arrange & Act
        var product = MockProduct(productName: "Test Product", sku: "SKU123", stockQuantity: 100);

        // Assert
        Assert.NotNull(product);
        Assert.Equal("SKU123", product.SKU);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal(100, product.Inventory.StockQuantity);
    }

    [Fact]
    public void CreateInventory_WithNegativeStock_ShouldThrowException()
    {
        // Arrange
        var negativeStock = -1;

        // Act
        var action = () => new ProductInventory(negativeStock);
        var exception = Assert.Throws<ArgumentOutOfRangeException>(action);

        // Assert
        Assert.Equal(
            "Stock quantity cannot be negative. (Parameter 'quantity')",
            exception.Message);
    }

    [Fact]
    public void CreateProduct_WithEmptySku_ShouldThrowException()
    {
        // Arrange
        var action = () => MockProduct(productName: "Test Product", sku: string.Empty, stockQuantity: 100);

        // Act
        var exception = Assert.Throws<ArgumentException>(action);

        // Assert
        Assert.Equal(
            "SKU cannot be null or empty. (Parameter 'sku')",
            exception.Message);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    public void CreateProduct_WithInvalidSkuLength_ShouldThrowException(string sku)
    {
        // Arrange
        var action = () => MockProduct(productName: "Test Product", sku: sku, stockQuantity: 100);

        // Act
        var exception = Assert.Throws<ArgumentException>(action);

        // Assert
        Assert.Contains("SKU", exception.Message);
    }

    [Fact]
    public void Products_WithSameInformation_ShouldReturnTrue()
    {
        // Arrange
        var product1 = MockProduct(productName: "Test Product", sku: "SKU123", stockQuantity: 100);
        var product2 = MockProduct(productName: "Test Product", sku: "SKU123", stockQuantity: 100);

        // Act
        var result = product1.HasSameInfo(product2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Products_WithDifferentInformation_ShouldReturnFalse()
    {
        // Arrange
        var product1 = MockProduct(productName: "Test Product", sku: "SKU123", stockQuantity: 100);
        var product2 = MockProduct(productName: "Different Product", sku: "SKU123", stockQuantity: 100);

        // Act
        var result = product1.HasSameInfo(product2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Touch_ShouldUpdateLastUpdateAt()
    {
        // Arrange
        var product = MockProduct(productName: "Test Product", sku: "SKU123", stockQuantity: 100);

        var originalLastUpdateAt = product.LastUpdateAt;

        Thread.Sleep(10);

        // Act
        product.Touch();

        // Assert
        Assert.True(product.LastUpdateAt > originalLastUpdateAt);
    }

    private Product MockProduct(string productName, string sku, int stockQuantity)
    {
        var productDetails = new ProductDetails(
            "Test Brand",
            1.5m,
            MeasureType.Kilogram);

        var productInventory = new ProductInventory(stockQuantity);

        return new Product(
            sku: sku,
            name: productName,
            description: "This is a test product.",
            price: 99.99m,
            category: ProductCategory.Salgados,
            details: productDetails,
            inventory: productInventory,
            createdAt: DateTime.UtcNow);
    }

}
