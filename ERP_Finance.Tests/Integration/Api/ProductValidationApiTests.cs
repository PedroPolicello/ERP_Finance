using System.Net;
using System.Net.Http.Json;
using ERP_Finance.DTOs.Product;
using ERP_Finance.Tests.Integration.Infrastructure;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Integration.Api;

public class ProductValidationApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductValidationApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostProduct_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();
        productDto.Name = string.Empty;

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
    public async Task PostProduct_WithEmptyDescription_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();
        productDto.Description = string.Empty;

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
    public async Task PostProduct_WithZeroPrice_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();
        productDto.Price = 0;

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
    public async Task PostProduct_WithZeroWeightOrVolume_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();
        productDto.WeightOrVolume = 0;

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
    public async Task PostProduct_WithInvalidCategory_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();
        productDto.Category = (ProductCategory)999;

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
    public async Task PostProduct_WithInvalidMeasureType_ShouldReturnBadRequest()
    {
        // Arrange
        var productDto = CreateProductDto();
        productDto.MeasureType = (MeasureType)999;

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
            Name = "Test Product",
            Description = "This is a test product.",
            Price = 99.99m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.5m,
            MeasureType = MeasureType.Kilogram
        };
    }
}