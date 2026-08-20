using ERP_Finance.DTOs.Product;
using ERP_Finance.Service;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Unit.Services;

public class ProductServiceTests
{
    [Fact]
    public void CreateProduct_WithValidInfo_ShouldCreateProductWithGeneratedSku()
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
        Assert.Matches(
            @"^\d{3}-\d{3}-\d{3}$",
            result.SKU);
    }

    [Fact]
    public void CreateProduct_WithNullDto_ShouldThrowException()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            productService.CreateProductService(null!));

        // Assert
        Assert.Equal(
            "productDTO",
            exception.ParamName);
    }

    [Fact]
    public void GetProduct_WithExistingId_ShouldReturnProduct()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var createdProduct = productService
            .CreateProductService(CreateProductDto());

        // Act
        var result = productService.GetProductService(
            createdProduct.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createdProduct.Id, result.Id);
        Assert.Equal(createdProduct.SKU, result.SKU);
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
            productService.GetProductService(nonExistingId));

        // Assert
        Assert.Equal(
            "Product not found.",
            exception.Message);
    }

    [Fact]
    public void UpdateProduct_WithExistingId_ShouldUpdateProductAndKeepSku()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var createdProduct = productService
            .CreateProductService(CreateProductDto());

        var originalSku = createdProduct.SKU;

        var updateDto = new UpdateProductDTO
        {
            Name = "Updated Product",
            Description = "This is an updated test product.",
            Price = 79.99m,
            Category = ProductCategory.Doces,
            BrandName = "Updated Brand",
            WeightOrVolume = 2.0m,
            MeasureType = MeasureType.Liter
        };

        // Act
        var result = productService.UpdateProductService(
            createdProduct.Id,
            updateDto);

        // Assert
        Assert.True(result);

        var updatedProduct = productService.GetProductService(
            createdProduct.Id);

        Assert.Equal("Updated Product", updatedProduct.Name);
        Assert.Equal(
            "This is an updated test product.",
            updatedProduct.Description);
        Assert.Equal(79.99m, updatedProduct.Price);
        Assert.Equal(ProductCategory.Doces, updatedProduct.Category);
        Assert.Equal("Updated Brand", updatedProduct.Details.BrandName);
        Assert.Equal(2.0m, updatedProduct.Details.WeightOrVolume);
        Assert.Equal(MeasureType.Liter, updatedProduct.Details.MeasureType);
        Assert.Equal(originalSku, updatedProduct.SKU);
    }

    [Fact]
    public void UpdateProduct_WithPartialData_ShouldKeepFieldsNotSent()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var createdProduct = productService.CreateProductService(
            CreateProductDto());

        var originalDescription = createdProduct.Description;
        var originalPrice = createdProduct.Price;
        var originalCategory = createdProduct.Category;
        var originalBrandName = createdProduct.Details.BrandName;
        var originalWeightOrVolume =
            createdProduct.Details.WeightOrVolume;
        var originalMeasureType =
            createdProduct.Details.MeasureType;

        var updateDto = new UpdateProductDTO
        {
            Name = "Only Name Updated"
        };

        // Act
        var result = productService.UpdateProductService(
            createdProduct.Id,
            updateDto);

        // Assert
        Assert.True(result);

        var updatedProduct = productService.GetProductService(
            createdProduct.Id);

        Assert.Equal("Only Name Updated", updatedProduct.Name);
        Assert.Equal(
            originalDescription,
            updatedProduct.Description);
        Assert.Equal(originalPrice, updatedProduct.Price);
        Assert.Equal(originalCategory, updatedProduct.Category);
        Assert.Equal(
            originalBrandName,
            updatedProduct.Details.BrandName);
        Assert.Equal(
            originalWeightOrVolume,
            updatedProduct.Details.WeightOrVolume);
        Assert.Equal(
            originalMeasureType,
            updatedProduct.Details.MeasureType);
    }

    [Fact]
    public void UpdateProduct_WithNullDto_ShouldThrowException()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);

        var createdProduct = productService.CreateProductService(
            CreateProductDto());

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            productService.UpdateProductService(
                createdProduct.Id,
                null!));

        // Assert
        Assert.Equal(
            "productDTO",
            exception.ParamName);
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
            productService.UpdateProductService(
                nonExistingId,
                updateDto));

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

        var createdProduct = productService.CreateProductService(
            CreateProductDto());

        // Act
        var result = productService.DeleteProductService(
            createdProduct.Id);

        // Assert
        Assert.True(result);
        Assert.Empty(fakeRepository.AllProducts);

        Assert.Throws<KeyNotFoundException>(() =>
            productService.GetProductService(
                createdProduct.Id));
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
            productService.DeleteProductService(nonExistingId));

        // Assert
        Assert.Equal(
            "Product not found.",
            exception.Message);
    }

    private static CreateProductDTO CreateProductDto(
        string name = "Test Product")
    {
        return new CreateProductDTO
        {
            Name = name,
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram
        };
    }
}