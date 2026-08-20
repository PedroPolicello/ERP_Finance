using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ERP_Finance.DTOs.Product;
using ERP_Finance.Tests.Integration.Infrastructure;
using ERP_Finance.Types;

namespace ERP_Finance.Tests.Integration.Api;

public class ProductSearchApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductSearchApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SearchProducts_WithMatchingName_ShouldReturnSimilarProducts()
    {
        // Arrange
        var matchingProduct = await CreateProductAsync(
            name: "Arroz Branco Tipo 1");

        await CreateProductAsync(
            name: "Arroz Integral");

        var nonMatchingProduct = await CreateProductAsync(
            name: "Feijao Carioca");

        // Act
        var response = await _client.GetAsync(
            "/api/Product/search?name=arroz");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var products = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            JsonValueKind.Array,
            products.ValueKind);

        var returnedIds = products
            .EnumerateArray()
            .Select(product =>
                product.GetProperty("id").GetGuid())
            .ToList();

        Assert.Contains(matchingProduct, returnedIds);

        Assert.DoesNotContain(
            nonMatchingProduct,
            returnedIds);
    }

    [Fact]
    public async Task SearchProducts_WithDifferentLetterCase_ShouldReturnMatchingProducts()
    {
        // Arrange
        var productId = await CreateProductAsync(
            name: "Macarrao Parafuso");

        // Act
        var response = await _client.GetAsync(
            "/api/Product/search?name=MACARRAO");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var products = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        var returnedIds = products
            .EnumerateArray()
            .Select(product =>
                product.GetProperty("id").GetGuid())
            .ToList();

        Assert.Contains(productId, returnedIds);
    }

    [Fact]
    public async Task SearchProducts_WithNameThatDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateProductAsync(
            name: "Produto Existente");

        // Act
        var response = await _client.GetAsync(
            "/api/Product/search?name=ProdutoInexistente");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var products = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            JsonValueKind.Array,
            products.ValueKind);

        Assert.Empty(
            products.EnumerateArray());
    }

    [Fact]
    public async Task SearchProducts_WithEmptyName_ShouldReturnEmptyList()
    {
        // Arrange
        await CreateProductAsync(
            name: "Produto Existente");

        // Act
        var response = await _client.GetAsync(
            "/api/Product/search?name=");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var products = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(
            JsonValueKind.Array,
            products.ValueKind);

        Assert.Empty(
            products.EnumerateArray());
    }

    private async Task<Guid> CreateProductAsync(string name)
    {
        var productDto = new CreateProductDTO
        {
            Name = name,
            Description = "Product created for product search testing.",
            Price = 19.90m,
            Category = ProductCategory.Salgados,
            BrandName = "Test Brand",
            WeightOrVolume = 1.0m,
            MeasureType = MeasureType.Kilogram
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Product",
            productDto);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var product = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        return product
            .GetProperty("id")
            .GetGuid();
    }
}