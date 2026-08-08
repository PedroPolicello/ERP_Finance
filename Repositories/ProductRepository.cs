using ERP_Finance.Models;

namespace ERP_Finance.Repositories;

public class ProductRepository : Interfaces.IProductRepository
{
    private List<Product> products = new List<Product>();

    public bool AddToRepository(Product product)
    {
        if (product == null) return false;

        if (ProductExists(product.Id))
            return false;

        products.Add(product);

        return true;
    }

    public bool UpdateInRepository(Product product)
    {
        if (product == null) return false;

        var existingProduct = GetProductById(product.Id);
        if (existingProduct == null) return false;

        existingProduct.Update(product.Name, product.Description, product.PriceByUnit, product.Category, product.StockInfo, product.LastUpdateAt);

        return true;
    }

    public bool RemoveFromRepository(Product product)
    {
        if(product == null) return false;

        if(ProductExists(product.Id))
        {
            products.Remove(product);
            return true;    
        }
            return false;
    }

    public IReadOnlyList<Product> AllProducts => products.AsReadOnly();
    public Product? GetProductById(int id) => products.FirstOrDefault(p => p.Id == id);

    private bool ProductExists(int id) => products.Any(p => p.Id == id);
}
