using ERP_Finance.Models;
using ERP_Finance.Types;
using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.Product;

public class CreateProductDTO
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;


    [Required]
    [StringLength(300, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;


    [Range(0.01, double.MaxValue)]
    public decimal PriceByUnit { get; set; }


    [EnumDataType(typeof(ProductCategory))]
    public ProductCategory Category { get; set; }


    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
}