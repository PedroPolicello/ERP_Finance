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
            product.SKU.ToLowerInvariant());

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

        product.Update(
            "Updated Product",
            "Updated description.",
            75.50m,
            ProductCategory.Doces,
            product.Details,
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
            75.50m,
            updatedProduct.Price);
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

    private static Product CreateProduct()
    {
        var details = new ProductDetails(
            "Test Brand",
            1.5m,
            MeasureType.Kilogram);


        return new Product(
            sku: $"SKU-{Guid.NewGuid():N}".ToUpperInvariant(),
            name: "Test Product",
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