using ERP_Finance.Models;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Tests.Fakes;

public class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public IReadOnlyList<Product> AllProducts => _products.AsReadOnly();

    public bool AddToRepository(Product product)
    {
        if (product == null) return false;

        _products.Add(product);

        return true;
    }

    public Product? GetProductById(Guid id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public Product? GetProductBySKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return null;

        var normalizedSku = sku.Trim().ToUpperInvariant();

        return _products.FirstOrDefault(p => p.SKU == normalizedSku);
    }

    public bool RemoveFromRepository(Product product)
    {
        if (product == null) return false;

        return _products.Remove(product);
    }

    public bool UpdateInRepository(Product product)
    {
        if (product == null)
            return false;

        var existingProduct = GetProductById(product.Id);

        if (existingProduct == null)
            return false;

        existingProduct.Update(
            product.Name,
            product.Description,
            product.Price,
            product.Category,
            product.Details,
            product.LastUpdateAt);

        return true;
    }
}