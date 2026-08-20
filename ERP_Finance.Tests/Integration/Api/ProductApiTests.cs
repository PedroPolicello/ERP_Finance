using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ERP_Finance.DTOs.Product;
using ERP_Finance.Tests.Integration.Infrastructure;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Integration.Api;

public class ProductApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var productDto = CreateProductDto();

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            productDto.SKU.ToUpperInvariant(),
            body.GetProperty("sku").GetString());

        Assert.Equal(
            productDto.Name,
            body.GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetProduct_WithExistingId_ShouldReturnOk()
    {
        // Arrange
        var productDto = CreateProductDto(
            name: "Product To Retrieve");

        var createdProduct =
            await CreateProductAsync(productDto);

        var productId = createdProduct
            .GetProperty("id")
            .GetGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/Product/{productId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            productId,
            body.GetProperty("id").GetGuid());

        Assert.Equal(
            productDto.SKU.ToUpperInvariant(),
            body.GetProperty("sku").GetString());

        Assert.Equal(
            productDto.Name,
            body.GetProperty("name").GetString());
    }

    [Fact]
    public async Task GetProduct_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync(
            $"/api/Product/{nonExistingId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task PatchProduct_WithExistingId_ShouldReturnOk()
    {
        // Arrange
        var productDto = CreateProductDto();

        var createdProduct =
            await CreateProductAsync(productDto);

        var productId = createdProduct
            .GetProperty("id")
            .GetGuid();

        var updateDto = new UpdateProductDTO
        {
            Name = "Updated Product",
            Description = "Updated description.",
            Price = 79.99m,
            Category = ProductCategory.Doces,
            BrandName = "Updated Brand",
            WeightOrVolume = 2.0m,
            MeasureType = MeasureType.Liter,
        };

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/Product/{productId}",
            updateDto);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var getResponse = await _client.GetAsync(
            $"/api/Product/{productId}");

        Assert.Equal(
            HttpStatusCode.OK,
            getResponse.StatusCode);

        var body = await getResponse.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            "Updated Product",
            body.GetProperty("name").GetString());

        Assert.Equal(
            "Updated description.",
            body.GetProperty("description").GetString());

        Assert.Equal(
            79.99m,
            body.GetProperty("price").GetDecimal());
    }

    [Fact]
    public async Task PatchProduct_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        var updateDto = new UpdateProductDTO
        {
            Name = "Updated Product",
            Price = 100m
        };

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/Product/{nonExistingId}",
            updateDto);

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WithExistingId_ShouldReturnNoContent()
    {
        // Arrange
        var productDto = CreateProductDto();

        var createdProduct =
            await CreateProductAsync(productDto);

        var productId = createdProduct
            .GetProperty("id")
            .GetGuid();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/Product/{productId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NoContent,
            response.StatusCode);

        var getResponse = await _client.GetAsync(
            $"/api/Product/{productId}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync(
            $"/api/Product/{nonExistingId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    private async Task<JsonElement> CreateProductAsync(
        CreateProductDTO productDto)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        return await response.Content
            .ReadFromJsonAsync<JsonElement>();
    }

    private static CreateProductDTO CreateProductDto(
        string? name = null)
    {
        return new CreateProductDTO
        {
            SKU = $"SKU-{Guid.NewGuid():N}",
            Name = name ?? "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
        };
    }
}