using ERP_Finance.Types;
using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.Tab;

public class CreateTabDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int TabNumber { get; set; }

    [Required]
    public ServiceType ServiceType { get; set; }

    [Range(1, int.MaxValue)]
    public int? TableNumber { get; set; }

    public string? Note { get; set; }

}
