using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.Table;

public class CreateTableDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int TableNumber { get; set; }
}
