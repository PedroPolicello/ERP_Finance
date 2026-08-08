using ERP_Finance.Types;

namespace ERP_Finance.Models;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal PriceByUnit { get; private set; }
    public ProductCategory Category { get; private set; }
    //public Image ProductImage { get; private set; } //Adicionar
    public ProductStockInfo StockInfo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdateAt { get; private set; }


    public Product(int id, string name, string description, decimal priceByUnit, ProductCategory category, ProductStockInfo stockInfo, DateTime createdAt)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(priceByUnit);
        ValidateCategory(category);
        ValidateStockInfo(stockInfo);


        Id = id;
        Name = name.Trim();
        Description = description.Trim();
        PriceByUnit = priceByUnit;
        Category = category;
        StockInfo = stockInfo;
        CreatedAt = createdAt;
        LastUpdateAt = createdAt;
    }

    public void Update(string name, string description, decimal priceByUnit, ProductCategory category, ProductStockInfo stockInfo, DateTime lastUpdateAt)
    {
        ValidateName(name);
        ValidateDescription(description);
        ValidatePrice(priceByUnit);
        ValidateCategory(category);
        ValidateStockInfo(stockInfo);

        Name = name.Trim();
        Description = description.Trim();
        PriceByUnit = priceByUnit;
        Category = category;
        StockInfo = stockInfo;
        LastUpdateAt = lastUpdateAt;
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

    private static void ValidateStockInfo(ProductStockInfo stockInfo)
    {
        if (stockInfo == null)
            throw new ArgumentNullException(nameof(stockInfo), "Stock information cannot be null.");

        if (stockInfo.StockQuantity < 0)
            throw new ArgumentOutOfRangeException(nameof(stockInfo.StockQuantity), "Stock quantity cannot be negative.");
    }

}


public class ProductStockInfo
{
    public int StockQuantity { get; private set; }

    public bool IsInStock => StockQuantity > 0;

    public ProductStockInfo(int stockQuantity)
    {
        SetStockQuantity(stockQuantity);
    }

    public void SetStockQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Stock quantity cannot be negative.");

        StockQuantity = quantity;
    }
}