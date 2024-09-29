using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendaPues.Shared.DTOs;

public class ProductDTO
{
    public int Id { get; set; }

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

    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Costo")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public decimal Cost { get; set; }

    [DisplayFormat(DataFormatString = "{0:P2}")]
    [Display(Name = "% Utilidad Deseado")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float DesiredProfit { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}")]
    [Display(Name = "Inventario")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float Stock { get; set; }

    public List<int>? ProductCategoryIds { get; set; }

    public List<string>? ProductImages { get; set; }
}