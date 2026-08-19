using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ERP_Finance.Tests.Integration;

public class ErrorResponseApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ErrorResponseApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProduct_WithNonExistingId_ShouldReturnErrorResponse()
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

        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Contains(
            "Product not found.",
            body.ToString());
    }

    [Fact]
    public async Task PatchProduct_WithNonExistingId_ShouldReturnErrorResponse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        var updateDto = new
        {
            Name = "Updated Product",
            Description = "Updated description.",
            Price = 100m,
            Category = 0,
            BrandName = "Updated Brand",
            WeightOrVolume = 1.0m,
            MeasureType = 0,
            StockQuantity = 100
        };

        // Act
        var response = await _client.PatchAsJsonAsync(
            $"/api/Product/{nonExistingId}",
            updateDto);

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);

        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Contains(
            "Product not found.",
            body.ToString());
    }

    [Fact]
    public async Task DeleteProduct_WithNonExistingId_ShouldReturnErrorResponse()
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

        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var body = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Contains(
            "Product not found.",
            body.ToString());
    }
}