using ERP_Finance.Data;
using ERP_Finance.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERP_Finance.Repositories;

public class ProductRepository : Interfaces.IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public bool AddToRepository(Product product)
    {
        if (product == null)
            return false;

        _context.Products.Add(product);
        var result = _context.SaveChanges();

        return result > 0;
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

        var result = _context.SaveChanges();

        return result > 0;
    }

    public bool RemoveFromRepository(Product product)
    {
        if (product == null)
            return false;

        var existingProduct = GetProductById(product.Id);

        if (existingProduct == null)
            return false;

        _context.Products.Remove(existingProduct);
        var result = _context.SaveChanges();

        return result > 0;
    }

    public IReadOnlyList<Product> AllProducts => _context.Products.AsNoTracking().ToList();

    public Product? GetProductById(Guid id) => _context.Products.FirstOrDefault(product => product.Id == id);

    public Product? GetProductBySKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return null;

        var normalizedSku = sku.Trim().ToUpperInvariant();

        return _context.Products.FirstOrDefault(product => product.SKU == normalizedSku);
    }

    public IReadOnlyList<Product> GetProductsByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return [];

        var normalizedName = name.Trim();

        return _context.Products
            .AsNoTracking()
            .Where(product =>
                EF.Functions.Like(
                    product.Name,
                    $"%{normalizedName}%"))
            .ToList();
    }
}