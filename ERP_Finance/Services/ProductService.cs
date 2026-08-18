using ERP_Finance.DTOs.Product;
using ERP_Finance.Models;
using ERP_Finance.Repositories.Interfaces;

namespace ERP_Finance.Service;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public AddProductResult AddProductService(CreateProductDTO productDTO)
    {
        if (productDTO == null)
            throw new ArgumentNullException(nameof(productDTO));

        var stockInfo = new ProductInventory(productDTO.StockQuantity);

        var productDetails = new ProductDetails(
            productDTO.BrandName,
            productDTO.WeightOrVolume,
            productDTO.MeasureType);

        var newProduct = new Product(
            productDTO.SKU,
            productDTO.Name,
            productDTO.Description,
            productDTO.Price,
            productDTO.Category,
            productDetails,
            stockInfo,
            DateTime.UtcNow);

        var existingProduct = _productRepository.GetProductBySKU(productDTO.SKU);
        if (existingProduct is not null)
        {
            if (!existingProduct.HasSameInfo(newProduct))
                throw new InvalidOperationException("A product with the same SKU and different information already exists.");

            existingProduct.Inventory.SetStockQuantity(existingProduct.Inventory.StockQuantity + productDTO.StockQuantity);

            existingProduct.Touch();

            var updated = _productRepository.UpdateInRepository(existingProduct);

            if (!updated)
                throw new InvalidOperationException("The existing product could not be updated.");

            return new AddProductResult(existingProduct, WasCreated: false);
        }

        var created = _productRepository.AddToRepository(newProduct);

        if (!created)
            throw new InvalidOperationException("The product could not be created.");

        return new AddProductResult(newProduct, WasCreated: true);
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
        var stockInfo = product.Inventory;

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

        if (productDTO.StockQuantity.HasValue)
            stockInfo.SetStockQuantity(productDTO.StockQuantity.Value);


        product.Update(name, description, price, category, productDetails, stockInfo, DateTime.UtcNow);

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
}
