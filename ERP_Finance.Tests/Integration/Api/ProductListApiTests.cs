using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ERP_Finance.DTOs.Product;
using ERP_Finance.Tests.Integration.Infrastructure;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Integration.Api;

public class ProductListApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductListApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOkWithProductList()
    {
        // Arrange
        var productDto = new CreateProductDTO
        {
            Name = "Product In List",
            Description = "Product created for list integration testing.",
            Price = 39.90m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.0m,
            MeasureType = MeasureType.Kilogram,
        };

        var postResponse = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        Assert.Equal(
            HttpStatusCode.Created,
            postResponse.StatusCode);

        // Act
        var response = await _client.GetAsync(
            "/api/Product");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            JsonValueKind.Array,
            body.ValueKind);
    }
}