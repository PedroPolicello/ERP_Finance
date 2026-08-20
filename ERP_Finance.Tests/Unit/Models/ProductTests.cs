using ERP_Finance.Models;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Unit.Models;

public class ProductTests
{
    [Fact]
    public void CreateProduct_WithValidData_ShouldCreateProduct()
    {
        // Arrange & Act
        var product = CreateProduct(
            productName: "Test Product",
            sku: "123-456-789");

        // Assert
        Assert.NotNull(product);
        Assert.Equal("123-456-789", product.SKU);
        Assert.Equal("Test Product", product.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123456789")]
    [InlineData("123-456-78")]
    [InlineData("123-456-7890")]
    [InlineData("ABC-DEF-GHI")]
    [InlineData("123 456 789")]
    [InlineData("123-456-78A")]
    public void CreateProduct_WithInvalidSkuFormat_ShouldThrowException(
        string sku)
    {
        // Arrange
        var action = () => CreateProduct(
            productName: "Test Product",
            sku: sku);

        // Act
        var exception = Assert.Throws<ArgumentException>(action);

        // Assert
        Assert.Contains("SKU", exception.Message);
    }

    [Fact]
    public void CreateProduct_WithSkuContainingSpaces_ShouldThrowException()
    {
        // Arrange
        var action = () => CreateProduct(
            productName: "Test Product",
            sku: " 123-456-789 ");

        // Act
        var exception = Assert.Throws<ArgumentException>(action);

        // Assert
        Assert.Contains("SKU", exception.Message);
    }

    [Fact]
    public void CreateProduct_ShouldTrimTextFields()
    {
        // Arrange & Act
        var product = CreateProduct(
            productName: "  Test Product  ",
            sku: "123-456-789",
            description: "  Test description.  ",
            brandName: "  Test Brand  ");

        // Assert
        Assert.Equal("Test Product", product.Name);
        Assert.Equal("Test description.", product.Description);
        Assert.Equal("Test Brand", product.Details.BrandName);
    }

    [Fact]
    public void UpdateProduct_WithValidData_ShouldUpdateFieldsAndKeepSku()
    {
        // Arrange
        var product = CreateProduct(
            productName: "Original Product",
            sku: "123-456-789");

        var originalSku = product.SKU;
        var originalCreatedAt = product.CreatedAt;

        var updatedDetails = new ProductDetails(
            "Updated Brand",
            2.0m,
            MeasureType.Liter);

        var lastUpdateAt = DateTime.UtcNow;

        // Act
        product.Update(
            "Updated Product",
            "Updated description.",
            79.99m,
            ProductCategory.Doces,
            updatedDetails,
            lastUpdateAt);

        // Assert
        Assert.Equal("Updated Product", product.Name);
        Assert.Equal("Updated description.", product.Description);
        Assert.Equal(79.99m, product.Price);
        Assert.Equal(ProductCategory.Doces, product.Category);
        Assert.Equal("Updated Brand", product.Details.BrandName);
        Assert.Equal(2.0m, product.Details.WeightOrVolume);
        Assert.Equal(MeasureType.Liter, product.Details.MeasureType);
        Assert.Equal(originalSku, product.SKU);
        Assert.Equal(originalCreatedAt, product.CreatedAt);
        Assert.Equal(lastUpdateAt, product.LastUpdateAt);
    }

    [Fact]
    public void Touch_ShouldUpdateLastUpdateAt()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddMinutes(-1);

        var product = CreateProduct(
            productName: "Test Product",
            sku: "123-456-789",
            createdAt: createdAt);

        var originalLastUpdateAt = product.LastUpdateAt;

        // Act
        product.Touch();

        // Assert
        Assert.True(product.LastUpdateAt > originalLastUpdateAt);
    }

    private static Product CreateProduct(
        string productName,
        string sku,
        string description = "This is a test product.",
        string brandName = "Test Brand",
        DateTime? createdAt = null)
    {
        var productDetails = new ProductDetails(
            brandName,
            1.5m,
            MeasureType.Kilogram);

        return new Product(
            sku: sku,
            name: productName,
            description: description,
            price: 99.99m,
            category: ProductCategory.Salgados,
            details: productDetails,
            createdAt: createdAt ?? DateTime.UtcNow);
    }
}