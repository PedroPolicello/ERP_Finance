using ERP_Finance.DTOs.Product;
using ERP_Finance.Models;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Service;

public class ProductService
{
    private IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public bool AddProductService(CreateProductDTO productDTO)
    {
        if (productDTO == null)
            throw new ArgumentNullException(nameof(productDTO));

        var stockInfo = new ProductStockInfo(productDTO.StockQuantity);

        var product = new Product(GenerateId(), productDTO.Name, productDTO.Description, productDTO.PriceByUnit, productDTO.Category, stockInfo, DateTime.UtcNow);

        return _productRepository.AddToRepository(product);
    }

    public bool UpdateProductService(int id, UpdateProductDTO productDTO)
    {

        if (productDTO == null)
            throw new ArgumentNullException(nameof(productDTO));

        var product = _productRepository.GetProductById(id);

        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        var name = product.Name;
        var description = product.Description;
        var price = product.PriceByUnit;
        var category = product.Category;
        var stockInfo = product.StockInfo;

        if (productDTO.Name is not null)
            name = productDTO.Name.Trim();

        if (productDTO.Description is not null)
            description = productDTO.Description.Trim();

        if (productDTO.PriceByUnit.HasValue)
            price = productDTO.PriceByUnit.Value;

        if (productDTO.Category.HasValue)
            category = productDTO.Category.Value;

        if (productDTO.StockQuantity.HasValue)
            stockInfo.SetStockQuantity(productDTO.StockQuantity.Value);


        product.Update(name, description, price, category, stockInfo, DateTime.UtcNow);

        return _productRepository.UpdateInRepository(product);
    }

    public bool DeleteProductService(int id)
    {
        var product = _productRepository.GetProductById(id);

        if (product == null)
            throw new Exception("Product not found.");

        return _productRepository.RemoveFromRepository(product);
    }

    public Product? GetProductService(int id) => _productRepository.GetProductById(id);

    public IReadOnlyList<Product> GetAllProductsService() => _productRepository.AllProducts;

    private int GenerateId()
    {
        if (_productRepository.AllProducts.Count == 0)
        {
            return 1;
        }

        return _productRepository.AllProducts.Max(
            product => product.Id) + 1;
    }

}
