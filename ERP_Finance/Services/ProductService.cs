using ERP_Finance.DTOs.Product;
using ERP_Finance.Helpers;
using ERP_Finance.Entities;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public Product CreateProductService(CreateProductDTO productDTO)
    {
        if (productDTO == null)
            throw new ArgumentNullException(nameof(productDTO));

        var productDetails = new ProductDetails(
            productDTO.BrandName,
            productDTO.WeightOrVolume,
            productDTO.MeasureType);


        // ========== Verificação de SKU ==========

        var sku = string.Empty;
        Product? existingProduct = null;
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            sku = SKUGenerator.GenerateSKU();
            existingProduct = _productRepository.GetProductBySKU(sku);

            if (existingProduct == null)
                break;
        }

        if (existingProduct != null)
            throw new InvalidOperationException($"Could not generate a unique SKU after {maxAttempts} attempts.");

        // ========== Verificação de SKU ==========

        var newProduct = new Product(
            sku,
            productDTO.Name,
            productDTO.Description,
            productDTO.Price,
            productDTO.Category,
            productDetails,
            DateTime.UtcNow);

        var created = _productRepository.AddToRepository(newProduct);

        if (!created)
            throw new InvalidOperationException("The product could not be created.");

        return newProduct;
    }

    public bool UpdateProductService(Guid id, UpdateProductDTO productDTO)
    {

        if (productDTO == null)
            throw new ArgumentNullException(nameof(productDTO));

        var product = _productRepository.GetProductById(id);

        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        var name = product.Name;
        var description = product.Description;
        var price = product.Price;
        var category = product.Category;
        var productDetails = product.Details;

        if (productDTO.Name is not null)
            name = productDTO.Name.Trim();

        if (productDTO.Description is not null)
            description = productDTO.Description.Trim();

        if (productDTO.Price.HasValue)
            price = productDTO.Price.Value;

        if (productDTO.Category.HasValue)
            category = productDTO.Category.Value;

        var brandName = product.Details.BrandName;
        var weightOrVolume = product.Details.WeightOrVolume;
        var measureType = product.Details.MeasureType;

        if (productDTO.BrandName is not null)
            brandName = productDTO.BrandName.Trim();

        if (productDTO.WeightOrVolume.HasValue)
            weightOrVolume = productDTO.WeightOrVolume.Value;

        if (productDTO.MeasureType.HasValue)
            measureType = productDTO.MeasureType.Value;

        productDetails.UpdateDetails(brandName, weightOrVolume, measureType);

        product.Update(name, description, price, category, productDetails, DateTime.UtcNow);

        return _productRepository.UpdateInRepository(product);
    }

    public bool DeleteProductService(Guid id)
    {
        var product = _productRepository.GetProductById(id);

        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        return _productRepository.RemoveFromRepository(product);
    }

    public Product GetProductService(Guid id)
    {
        var product = _productRepository.GetProductById(id);

        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        return product;
    }

    public IReadOnlyList<Product> GetAllProductsService() => _productRepository.AllProducts;

    public IReadOnlyList<Product> GetProductsByNameService(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return [];

        return _productRepository.GetProductsByName(name);
    }
}
