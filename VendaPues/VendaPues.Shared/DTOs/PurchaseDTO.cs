using System.ComponentModel.DataAnnotations;

namespace VendaPues.Shared.DTOs;

public class PurchaseDTO
{
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
    [Display(Name = "Fecha")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateTime Date { get; set; }

    public int SupplierId { get; set; }

    [DataType(DataType.MultilineText)]
    [Display(Name = "Comentarios")]
    public string? Remarks { get; set; }

    public List<PurchaseDetailDTO>? PurchaseDetails { get; set; }
}