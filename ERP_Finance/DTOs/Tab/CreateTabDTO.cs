using ERP_Finance.Types;
using System.ComponentModel.DataAnnotations;

namespace ERP_Finance.DTOs.Tab;

public class CreateTabDTO
{
    [Required]
    public ServiceType ServiceType { get; set; }

    public Guid? TableId { get; set; }

    public string? Note { get; set; }

}
