using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.OrderItem;

public class UpdateOrderItemDTO
{
    [Range(0.01, double.MaxValue)]
    public decimal? Quantity { get; set; }

    public string? Note { get; set; }
    public bool UpdateNoteField { get; set; } // ou outra forma explícita de sinalizar "quero limpar a nota"
}
