using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.Tab;

public class CancelTabDTO
{
    [Required]
    public string CancellationReason { get; set; } = string.Empty;
}