using ERP_Finance.Entities;

namespace ERP_Finance.Repositories.Interfaces;

public interface IProductRepository
{
    bool AddToRepository(Product product);
    bool UpdateInRepository(Product product);
    bool RemoveFromRepository(Product product);
    IReadOnlyList<Product> AllProducts { get; }
    Product? GetProductById(Guid id);
    Product? GetProductBySKU(string sku);
    IReadOnlyList<Product> GetProductsByName(string name);
}
