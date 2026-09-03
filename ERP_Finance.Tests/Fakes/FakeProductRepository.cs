using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Tests.Fakes;

public class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public IReadOnlyList<Product> AllProducts => _products.AsReadOnly();

    public bool AddToRepository(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        _products.Add(product);

        return true;
    }

    public Product? GetProductById(Guid id)
    {
        return _products.FirstOrDefault(product =>
            product.Id == id);
    }

    public Product? GetProductBySKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return null;

        var normalizedSku = sku.Trim().ToUpperInvariant();

        return _products.FirstOrDefault(product =>
            product.SKU == normalizedSku);
    }

    public bool RemoveFromRepository(Product product)
    {
        if (product is null)
            return false;

        return _products.Remove(product);
    }

    public bool UpdateInRepository(Product product)
    {
        if (product is null)
            return false;

        var existingProduct = GetProductById(product.Id);

        return existingProduct is not null;
    }

    public IReadOnlyList<Product> GetProductsByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return [];

        var normalizedName = name.Trim();

        return _products
            .Where(product =>
                product.Name.Contains(
                    normalizedName,
                    StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }
}