using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendaPues.Shared.DTOs;

public class PurchaseDetailDTO
{
    [DataType(DataType.MultilineText)]
    [Display(Name = "Comentarios")]
    public string? Remarks { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int ProductId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Costo")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Cost { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}")]
    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float Quantity { get; set; }
}