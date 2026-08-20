using ERP_Finance.Data;
using ERP_Finance.Models;
using ERP_Finance.Repositories;
using ERP_Finance.Types;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ERP_Finance.Tests.Integration.Repositories;

public class ProductRepositoryTests
    : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public ProductRepositoryTests()
    {
        _connection = new SqliteConnection(
            "Data Source=:memory:");

        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateContext();

        context.Database.EnsureCreated();
    }

    [Fact]
    public void AddToRepository_WithValidProduct_ShouldPersistProduct()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var product = CreateProduct();

        // Act
        var result = repository.AddToRepository(product);

        // Assert
        Assert.True(result);
        Assert.Single(repository.AllProducts);

        var savedProduct = repository
            .GetProductById(product.Id);

        Assert.NotNull(savedProduct);
        Assert.Equal(
            product.SKU,
            savedProduct.SKU);
    }

    [Fact]
    public void GetProductById_WithExistingId_ShouldReturnProduct()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var product = CreateProduct();

        repository.AddToRepository(product);

        // Act
        var result = repository.GetProductById(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.SKU, result.SKU);
    }

    [Fact]
    public void GetProductById_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        // Act
        var result = repository.GetProductById(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetProductBySKU_WithExistingSku_ShouldReturnProduct()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var product = CreateProduct();

        repository.AddToRepository(product);

        // Act
        var result = repository.GetProductBySKU(
            $"  {product.SKU.ToLowerInvariant()}  ");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.SKU, result.SKU);
    }

    [Fact]
    public void GetProductBySKU_WithEmptySku_ShouldReturnNull()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        // Act
        var result = repository.GetProductBySKU(
            string.Empty);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UpdateInRepository_WithExistingProduct_ShouldPersistChanges()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var product = CreateProduct();

        repository.AddToRepository(product);

        var updatedDetails = new ProductDetails(
            "Updated Brand",
            2.0m,
            MeasureType.Liter);

        product.Update(
            "Updated Product",
            "Updated description.",
            75.50m,
            ProductCategory.Doces,
            updatedDetails,
            DateTime.UtcNow);

        // Act
        var result = repository.UpdateInRepository(product);

        // Assert
        Assert.True(result);

        var updatedProduct = repository
            .GetProductById(product.Id);

        Assert.NotNull(updatedProduct);
        Assert.Equal(
            "Updated Product",
            updatedProduct.Name);

        Assert.Equal(
            "Updated description.",
            updatedProduct.Description);

        Assert.Equal(
            75.50m,
            updatedProduct.Price);

        Assert.Equal(
            ProductCategory.Doces,
            updatedProduct.Category);

        Assert.Equal(
            "Updated Brand",
            updatedProduct.Details.BrandName);

        Assert.Equal(
            2.0m,
            updatedProduct.Details.WeightOrVolume);

        Assert.Equal(
            MeasureType.Liter,
            updatedProduct.Details.MeasureType);
    }

    [Fact]
    public void UpdateInRepository_WithNonExistingProduct_ShouldReturnFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var product = CreateProduct();

        // Act
        var result = repository.UpdateInRepository(product);

        // Assert
        Assert.False(result);
        Assert.Empty(repository.AllProducts);
    }

    [Fact]
    public void RemoveFromRepository_WithExistingProduct_ShouldDeleteProduct()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var product = CreateProduct();

        repository.AddToRepository(product);

        // Act
        var result = repository.RemoveFromRepository(product);

        // Assert
        Assert.True(result);
        Assert.Empty(repository.AllProducts);

        Assert.Null(
            repository.GetProductById(product.Id));
    }

    [Fact]
    public void RemoveFromRepository_WithNonExistingProduct_ShouldReturnFalse()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var product = CreateProduct();

        // Act
        var result = repository.RemoveFromRepository(product);

        // Assert
        Assert.False(result);
        Assert.Empty(repository.AllProducts);
    }

    [Fact]
    public void GetProductsByName_WithMatchingName_ShouldReturnSimilarProducts()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var matchingProduct = CreateProduct(
            name: "Arroz Branco Tipo 1");

        var anotherMatchingProduct = CreateProduct(
            name: "Arroz Integral");

        var nonMatchingProduct = CreateProduct(
            name: "Feijao Carioca");

        repository.AddToRepository(matchingProduct);
        repository.AddToRepository(anotherMatchingProduct);
        repository.AddToRepository(nonMatchingProduct);

        // Act
        var result = repository.GetProductsByName("arroz");

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Contains(
            result,
            product => product.Id == matchingProduct.Id);

        Assert.Contains(
            result,
            product => product.Id == anotherMatchingProduct.Id);

        Assert.DoesNotContain(
            result,
            product => product.Id == nonMatchingProduct.Id);
    }

    [Fact]
    public void GetProductsByName_WithDifferentLetterCase_ShouldReturnMatchingProducts()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        var product = CreateProduct(
            name: "Macarrao Parafuso");

        repository.AddToRepository(product);

        // Act
        var result = repository.GetProductsByName("MACARRAO");

        // Assert
        Assert.Single(result);

        Assert.Equal(
            product.Id,
            result[0].Id);
    }

    [Fact]
    public void GetProductsByName_WithEmptyName_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = CreateContext();
        var repository = new ProductRepository(context);

        repository.AddToRepository(
            CreateProduct(name: "Produto Existente"));

        // Act
        var result = repository.GetProductsByName("   ");

        // Assert
        Assert.Empty(result);
    }

    private static Product CreateProduct(string name = "Test Product")
    {
        var details = new ProductDetails(
            "Test Brand",
            1.5m,
            MeasureType.Kilogram);

        var randomNumber = Random.Shared.Next(
            0,
            1_000_000_000);

        var sku = randomNumber.ToString("000-000-000");

        return new Product(
            sku: sku,
            name: name,
            description: "This is a test product.",
            price: 99.99m,
            category: ProductCategory.Salgados,
            details: details,
            createdAt: DateTime.UtcNow);
    }

    private AppDbContext CreateContext()
    {
        return new AppDbContext(_options);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}