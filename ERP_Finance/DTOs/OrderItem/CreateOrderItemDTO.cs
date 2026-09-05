using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.OrderItem;

public class CreateOrderItemDTO
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    public string? Note { get; set; }
}
