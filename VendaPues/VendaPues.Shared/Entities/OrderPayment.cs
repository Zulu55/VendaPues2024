using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendaPues.Shared.Entities;

public class OrderPayment
{
    public int Id { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
    [Display(Name = "Fecha")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public DateTime Date { get; set; }

    public Order? Order { get; set; }

    public int OrderId { get; set; }

    [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "Debes ingresar una dirección de correo válida.")]
    public string Email { get; set; } = null!;

    public Bank? Bank { get; set; }

    public int BankId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Valor")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Value { get; set; }

    [Display(Name = "Referencia")]
    [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    public string Reference { get; set; } = null!;
}