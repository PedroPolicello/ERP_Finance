using ERP_Finance.DTOs.Product;
using ERP_Finance.Models;
using ERP_Finance.Service;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Unit.Services;

public class ProductServiceTests
{
    [Fact]
    public void AddProduct_WithValidInfo_ShouldCreateProduct()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var productDto = CreateProductDto();

        // Act
        var result = productService.CreateProductService(productDto);

        // Assert
        Assert.NotNull(result);
        Assert.Single(fakeRepository.AllProducts);
    }

    [Fact]
    public void AddProduct_WithExistingSkuAndDifferentInformation_ShouldThrowException()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var firstDto = CreateProductDto();

        var secondDto = CreateProductDto(
            name: "Another Product");

        // Act
        productService.CreateProductService(firstDto);

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            productService.CreateProductService(secondDto);
        });

        // Assert
        Assert.Single(fakeRepository.AllProducts);
        Assert.Equal(
            "A product with the same SKU and different information already exists.",
            exception.Message);
    }

    [Fact]
    public void GetProduct_WithExistingId_ShouldReturnProduct()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var addResult =
            productService.CreateProductService(
                CreateProductDto());

        var productId = addResult.Id;

        // Act
        var result =
            productService.GetProductService(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("SKU123", result.SKU);
    }

    [Fact]
    public void GetProduct_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var nonExistingId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<KeyNotFoundException>(() =>
        {
            productService.GetProductService(nonExistingId);
        });

        // Assert
        Assert.Equal(
            "Product not found.",
            exception.Message);
    }

    [Fact]
    public void UpdateProduct_WithExistingId_ShouldUpdateProduct()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var addResult =
            productService.CreateProductService(
                CreateProductDto());

        var productId = addResult.Id;

        var updateDto = new UpdateProductDTO
        {
            Name = "Updated Product",
            Description = "This is an updated test product.",
            Price = 79.99m,
            Category = ProductCategory.Doces,
            BrandName = "Updated Brand",
            WeightOrVolume = 2.0m,
            MeasureType = MeasureType.Liter,
        };

        // Act
        var result =
            productService.UpdateProductService(
                productId,
                updateDto);

        // Assert
        Assert.True(result);

        var updatedProduct =
            productService.GetProductService(productId);

        Assert.Equal(
            "Updated Product",
            updatedProduct.Name);

        Assert.Equal(
            "This is an updated test product.",
            updatedProduct.Description);

        Assert.Equal(
            79.99m,
            updatedProduct.Price);
    }

    [Fact]
    public void UpdateProduct_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var nonExistingId = Guid.NewGuid();

        var updateDto = new UpdateProductDTO
        {
            Name = "Updated Product",
            Price = 100m
        };

        // Act
        var exception = Assert.Throws<KeyNotFoundException>(() =>
        {
            productService.UpdateProductService(
                nonExistingId,
                updateDto);
        });

        // Assert
        Assert.Equal(
            "Product not found.",
            exception.Message);
    }

    [Fact]
    public void DeleteProduct_WithExistingId_ShouldDeleteProduct()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var addResult =
            productService.CreateProductService(
                CreateProductDto());

        var productId = addResult.Id;

        // Act
        var result =
            productService.DeleteProductService(productId);

        // Assert
        Assert.True(result);
        Assert.Empty(fakeRepository.AllProducts);

        Assert.Throws<KeyNotFoundException>(() =>
        {
            productService.GetProductService(productId);
        });
    }

    [Fact]
    public void DeleteProduct_WithNonExistingId_ShouldThrowException()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var nonExistingId = Guid.NewGuid();

        // Act
        var exception = Assert.Throws<KeyNotFoundException>(() =>
        {
            productService.DeleteProductService(nonExistingId);
        });

        // Assert
        Assert.Equal(
            "Product not found.",
            exception.Message);
    }

    private static CreateProductDTO CreateProductDto(
        string name = "Test Product",
        int stockQuantity = 100)
    {
        return new CreateProductDTO
        {
            Name = name,
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
        };
    }
}