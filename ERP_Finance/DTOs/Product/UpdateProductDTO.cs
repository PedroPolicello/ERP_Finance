using System.ComponentModel.DataAnnotations;
using ERP_Finance.Models;
using ERP_Finance.Types;

namespace ERP_Finance.DTOs.Product;

public class UpdateProductDTO
{
    [StringLength(50, MinimumLength = 1)]
    public string? Name { get; set; }


    [StringLength(300, MinimumLength = 1)]
    public string? Description { get; set; }


    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }


    [EnumDataType(typeof(ProductCategory))]
    public ProductCategory? Category { get; set; }


    [StringLength(100, MinimumLength = 1)]
    public string? BrandName { get; set; }


    [Range(0.01, double.MaxValue)]
    public decimal? WeightOrVolume { get; set; }


    [EnumDataType(typeof(MeasureType))]
    public MeasureType? MeasureType { get; set; }

}