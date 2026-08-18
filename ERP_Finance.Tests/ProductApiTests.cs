using ERP_Finance.DTOs.Product;
using ERP_Finance.Types;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ERP_Finance.Tests.Integration;

public class ProductApiTests : IClassFixture<CustomWebApplicationFactory>
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
        var sku = $"SKU-{Guid.NewGuid():N}";

        var productDto = new CreateProductDTO
        {
            SKU = sku,
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
        var response = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            sku.ToUpperInvariant(),
            body.GetProperty("sku").GetString());

        Assert.Equal(
            "Test Product",
            body.GetProperty("name").GetString());
    }


    [Fact]
    public async Task GetProduct_WithExistingId_ShouldReturnOk()
    {
        // Arrange
        var sku = $"SKU-{Guid.NewGuid():N}";

        var productDto = new CreateProductDTO
        {
            SKU = sku,
            Name = "Product To Retrieve",
            Description = "Product created for integration testing.",
            Price = 49.90m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.0m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 25
        };

        var postResponse = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        postResponse.EnsureSuccessStatusCode();

        var createdProduct = await postResponse.Content.ReadFromJsonAsync<JsonElement>();

        var productId = createdProduct.GetProperty("id").GetGuid();

        // Act
        var response = await _client.GetAsync($"/api/Product/{productId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            productId,
            body.GetProperty("id").GetGuid());

        Assert.Equal(
            sku.ToUpperInvariant(),
            body.GetProperty("sku").GetString());

        Assert.Equal(
            "Product To Retrieve",
            body.GetProperty("name").GetString());
    }
}