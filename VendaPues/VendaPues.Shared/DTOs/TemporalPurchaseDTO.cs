using System.ComponentModel.DataAnnotations;

namespace VendaPues.Shared.DTOs;

public class TemporalPurchaseDTO
{
    public int Id { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
    [Display(Name = "Fecha")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public int SupplierId { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}")]
    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float Quantity { get; set; } = 1;

    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Costo")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Cost { get; set; }

    public string RemarksGeneral { get; set; } = string.Empty;

    public string RemarksDetail { get; set; } = string.Empty;
}