using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendaPues.Shared.Entities;

public class OrderDetail
{
    public int Id { get; set; }

    public Order? Order { get; set; }

    public int OrderId { get; set; }

    [DataType(DataType.MultilineText)]
    [Display(Name = "Comentarios")]
    public string? Remarks { get; set; }

    public Product? Product { get; set; }

    public int ProductId { get; set; }

    [Display(Name = "Nombre")]
    [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public string Name { get; set; } = null!;

    [DataType(DataType.MultilineText)]
    [Display(Name = "Descripción")]
    [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
    public string Description { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Precio")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Price { get; set; }

    [Display(Name = "Imagén")]
    public string Image { get; set; } = null!;

    [DisplayFormat(DataFormatString = "{0:N2}")]
    [Display(Name = "Cantidad")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float Quantity { get; set; }

    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Valor")]
    public decimal Value => (decimal)Quantity * Price;
}