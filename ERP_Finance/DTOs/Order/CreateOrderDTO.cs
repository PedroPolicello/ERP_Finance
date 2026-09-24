using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.Order;

public class CreateOrderDTO
{
    [Required]
    public Guid TabId { get; set; }

    public string? Note { get; set; }
}
