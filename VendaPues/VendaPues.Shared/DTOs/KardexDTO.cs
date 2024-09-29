using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VendaPues.Shared.Enums;

namespace VendaPues.Shared.DTOs;

public class KardexDTO
{
    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
    [Display(Name = "Fecha")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateTime Date { get; set; }

    public int ProductId { get; set; }

    [Display(Name = "Tipo Movimiento")]
    public KardexType KardexType { get; set; }

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