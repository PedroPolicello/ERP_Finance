using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.Order;

public class CreateOrderDTO
{
    [Required, Range(1, int.MaxValue)]
    public int OrderNumber { get; set; }

    [Required]
    public Guid TabId { get; set; }

    public string? Note { get; set; }
}
