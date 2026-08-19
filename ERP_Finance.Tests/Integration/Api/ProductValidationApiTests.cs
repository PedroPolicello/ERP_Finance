using System.Net;
using System.Net.Http.Json;

using ERP_Finance.DTOs.Product;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Integration;

public class ProductValidationApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductValidationApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostProduct_WithEmptySku_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();

        productDto.SKU = string.Empty;

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_WithInvalidSkuLength_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();

        productDto.SKU = "AB";

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_WithNegativeStock_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();

        productDto.StockQuantity = -1;

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    private static CreateProductDTO CreateProductDto()
    {
        return new CreateProductDTO
        {
            SKU = $"SKU-{Guid.NewGuid():N}",
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram,
            StockQuantity = 100
        };
    }
}