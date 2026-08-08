using System.ComponentModel.DataAnnotations;
using ERP_Finance.Models;
using ERP_Finance.Types;

namespace ERP_Finance.DTOs.Product;

public class UpdateProductDTO
{
    [StringLength(50, MinimumLength = 1)]
    public string? Name { get; set; }


    [StringLength(200, MinimumLength = 1)]
    public string? Description { get; set; }


    [Range(0.01, double.MaxValue)]
    public decimal? PriceByUnit { get; set; }


    [EnumDataType(typeof(ProductCategory))]
    public ProductCategory? Category { get; set; }


    [Range(0, int.MaxValue)]
    public int? StockQuantity { get; set; }

}