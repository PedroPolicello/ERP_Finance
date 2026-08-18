using ERP_Finance.DTOs.Product;
using ERP_Finance.Models;
using ERP_Finance.Repositories.Interfaces;
using ERP_Finance.Service;
using ERP_Finance.Types;


namespace ERP_Finance.Tests;

public class ProductServiceTests
{
    [Fact]
    public void AddProduct_WithNewSku_ShouldCreateProduct()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var productDto = new CreateProductDTO
        {
            SKU = "SKU123",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };

        // Act
        var result = productService.AddProductService(productDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.WasCreated);
        Assert.Equal("SKU123", result.Product.SKU);
        Assert.Single(fakeRepository.AllProducts);
    }

    [Fact]
    public void AddProduct_WithExistingSkuAndSameInformation_ShouldAddStock()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var productDto1 = new CreateProductDTO
        {
            SKU = "SKU123",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };

        var productDto2 = new CreateProductDTO
        {
            SKU = "SKU123",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 50
        };


        // Act
        var firstAddition = productService.AddProductService(productDto1);
        var originalLastUpdateAt = firstAddition.Product.LastUpdateAt;

        Thread.Sleep(10);

        var result = productService.AddProductService(productDto2);


        // Assert
        Assert.False(result.WasCreated);
        Assert.Single(fakeRepository.AllProducts);
        Assert.Equal(150, result.Product.Inventory.StockQuantity);
        Assert.True(result.Product.LastUpdateAt > originalLastUpdateAt);
    }

    [Fact]
    public void AddProduct_WithExistingSkuAndDifferentInformation_ShouldThrowException()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var productDto1 = new CreateProductDTO
        {
            SKU = "SKU123",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };

        var productDto2 = new CreateProductDTO
        {
            SKU = "SKU123",
            Name = "Another Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };

        // Act
        productService.AddProductService(productDto1);
        var action = () => productService.AddProductService(productDto2);

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(action);
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
        var productDto = new CreateProductDTO
        {
            SKU = "SKU123",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };

        var addResult = productService.AddProductService(productDto);
        var productId = addResult.Product.Id;

        // Act
        var result = productService.GetProductService(productId);

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
        var action = () => productService.GetProductService(nonExistingId);

        // Assert
        var exception = Assert.Throws<KeyNotFoundException>(action);
        Assert.Equal("Product not found.", exception.Message);
    }

    [Fact]
    public void UpdateProduct_WithExistingId_ShouldUpdateProduct()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var productDto = new CreateProductDTO
        {
            SKU = "SKU123",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };

        var addResult = productService.AddProductService(productDto);
        var productId = addResult.Product.Id;

        var updateDto = new UpdateProductDTO
        {
            Name = "Updated Product",
            Description = "This is an updated test product.",
            Price = 79.99m,
            Category = ProductCategory.Doces,
            BrandName = "Updated Brand",
            WeightOrVolume = 2.0m,
            MeasureType = MeasureType.Liter,
            StockQuantity = 150
        };

        // Act
        var result = productService.UpdateProductService(productId, updateDto);

        // Assert
        Assert.True(result);
        var updatedProduct = productService.GetProductService(productId);
        Assert.Equal("Updated Product", updatedProduct.Name);
        Assert.Equal("This is an updated test product.", updatedProduct.Description);
        Assert.Equal(79.99m, updatedProduct.Price);
        Assert.Equal(150, updatedProduct.Inventory.StockQuantity);
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
        Assert.Equal("Product not found.", exception.Message);
    }

    [Fact]
    public void DeleteProduct_WithExistingId_ShouldDeleteProduct()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var productDto = new CreateProductDTO
        {
            SKU = "SKU123",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };

        var addResult = productService.AddProductService(productDto);
        var productId = addResult.Product.Id;

        // Act
        var result = productService.DeleteProductService(productId);

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
        Assert.Equal("Product not found.", exception.Message);
    }


    [Fact]
    public void AddProduct_WithEmptySku_ShouldThrowException()
    {
        // Arrange
        var fakeRepository = new Fakes.FakeProductRepository();
        var productService = new ProductService(fakeRepository);
        var productDto = new CreateProductDTO
        {
            SKU = "",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
        {
            productService.AddProductService(productDto);
        });

        // Assert
        Assert.Contains("SKU", exception.Message);
        Assert.Empty(fakeRepository.AllProducts);
    }

}
