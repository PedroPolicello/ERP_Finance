using ERP_Finance.Helpers;
using ERP_Finance.Types;

namespace ERP_Finance.Models;

public class Product
{
    public Guid Id { get; private set; }
    public string SKU { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public ProductCategory Category { get; private set; }
    public string? ImageUrl { get; private set; }
    public ProductDetails Details { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdateAt { get; private set; }

    private Product()
    { }

    public Product(string sku, string name, string description, decimal price, ProductCategory category, ProductDetails details, DateTime createdAt)
    {
        ValidateSKU(sku);
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(price);
        ValidateCategory(category);
        ValidateDetails(details);

        Id = Guid.NewGuid();
        SKU = sku.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        Category = category;
        Details = details;
        CreatedAt = createdAt;
        LastUpdateAt = createdAt;

        //SKU = SKUGenerator.GenerateSKU(Name, Details); // SKU é criado pelo usuario
    }

    public void Update(string name, string description, decimal priceByUnit, ProductCategory category, ProductDetails details, DateTime lastUpdateAt)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(priceByUnit);
        ValidateCategory(category);
        ValidateDetails(details);

        Name = name.Trim();
        Description = description.Trim();
        Price = priceByUnit;
        Category = category;
        Details = details;
        LastUpdateAt = lastUpdateAt;

        //SKU = SKUGenerator.GenerateSKU(Name, Details); // SKU é criado pelo usuario
    }

    public void Touch()
    {
        LastUpdateAt = DateTime.UtcNow;
    }

    public bool HasSameInfo(Product other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return SKU.Equals(other.SKU, StringComparison.OrdinalIgnoreCase)
            && Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase)
            && Description.Equals(other.Description, StringComparison.Ordinal)
            && Price == other.Price
            && Category == other.Category

            && Details.BrandName.Equals(other.Details.BrandName, StringComparison.OrdinalIgnoreCase)
            && Details.WeightOrVolume == other.Details.WeightOrVolume
            && Details.MeasureType == other.Details.MeasureType;
    }

    private static void ValidateSKU(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new ArgumentException("SKU cannot be null or empty.", nameof(sku));

        if (sku.Length < 3 || sku.Length > 50)
            throw new ArgumentException("SKU must be between 3 and 50 characters.", nameof(sku));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be null or empty.", nameof(name));
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be null or empty.", nameof(description));
    }

    private static void ValidatePrice(decimal price)
    {
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Product price must be greater than zero.");
    }

    private static void ValidateCategory(ProductCategory category)
    {
        if (!Enum.IsDefined(typeof(ProductCategory), category))
            throw new ArgumentException("Invalid product category.", nameof(category));
    }

    private static void ValidateDetails(ProductDetails details)
    {
        if (details == null)
            throw new ArgumentNullException(nameof(details), "Product details cannot be null.");

        details.ValidateInfo(details.BrandName, details.WeightOrVolume, details.MeasureType);
    }
}

public class ProductDetails
{
    public string BrandName { get; private set; } = string.Empty;
    public decimal WeightOrVolume { get; private set; }
    public MeasureType MeasureType { get; private set; }

    public ProductDetails(string brandName, decimal weightOrVolume, MeasureType measureType)
    {
        UpdateDetails(brandName, weightOrVolume, measureType);
    }

    public void UpdateDetails(string brandName, decimal weightOrVolume, MeasureType measureType)
    {
        ValidateInfo(brandName, weightOrVolume, measureType);

        BrandName = brandName;
        WeightOrVolume = weightOrVolume;
        MeasureType = measureType;
    }

    public void ValidateInfo(string brandName, decimal weightOrVolume, MeasureType measureType)
    {
        ValidateBrandName(brandName);
        ValidateWeight(weightOrVolume);
        ValidateMeasureType(measureType);
    }

    private static void ValidateBrandName(string brandName)
    {
        if (string.IsNullOrWhiteSpace(brandName))
            throw new ArgumentException("Brand name cannot be null or empty.", nameof(brandName));
    }

    private static void ValidateWeight(decimal weightOrVolume)
    {
        if (weightOrVolume <= 0)
            throw new ArgumentOutOfRangeException(nameof(weightOrVolume), "WeightOrVolume must be greater than zero.");
    }

    private static void ValidateMeasureType(MeasureType mesureType)
    {
        if (!Enum.IsDefined(typeof(MeasureType), mesureType))
            throw new ArgumentException("Invalid measure type.", nameof(mesureType));
    }

}

//public class ProductInventory
//{
//    public int StockQuantity { get; private set; }

//    public bool IsInStock => StockQuantity > 0;

//    public ProductInventory(int stockQuantity)
//    {
//        SetStockQuantity(stockQuantity);
//    }

//    public void SetStockQuantity(int quantity)
//    {
//        if (quantity < 0)
//            throw new ArgumentOutOfRangeException(nameof(quantity), "Stock quantity cannot be negative.");

//        StockQuantity = quantity;
//    }
//}
