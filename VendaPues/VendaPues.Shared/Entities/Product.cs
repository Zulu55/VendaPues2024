using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendaPues.Shared.Entities;

public class Product
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

    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Utilidad")]
    public decimal Profit => Price - Cost;

    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Valor a costo")]
    public decimal CostValue => Cost * (decimal)Stock;

    [DisplayFormat(DataFormatString = "{0:C2}")]
    [Display(Name = "Valor a precio")]
    public decimal PriceValue => Price * (decimal)Stock;

    [DisplayFormat(DataFormatString = "{0:P2}")]
    [Display(Name = "% Utilidad Real")]
    public float RealProfit => Cost == 0 ? 0 : (float)(Profit / Cost);

    [DisplayFormat(DataFormatString = "{0:P2}")]
    [Display(Name = "% Utilidad Deseado")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float DesiredProfit { get; set; }

    [DisplayFormat(DataFormatString = "{0:N2}")]
    [Display(Name = "Inventario")]
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    public float Stock { get; set; }

    public ICollection<ProductCategory>? ProductCategories { get; set; }

    [Display(Name = "Categorías")]
    public int ProductCategoriesNumber => ProductCategories == null || ProductCategories.Count == 0 ? 0 : ProductCategories.Count;

    public ICollection<ProductImage>? ProductImages { get; set; }

    [Display(Name = "Imágenes")]
    public int ProductImagesNumber => ProductImages == null || ProductImages.Count == 0 ? 0 : ProductImages.Count;

    [Display(Name = "Imagén")]
    public string MainImage => ProductImages == null || ProductImages.Count == 0 ? string.Empty : ProductImages.FirstOrDefault()!.Image;

    public ICollection<TemporalOrder>? TemporalOrders { get; set; }

    public ICollection<TemporalPurchase>? TemporalPurchases { get; set; }

    public ICollection<OrderDetail>? OrderDetails { get; set; }

    public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }

    public ICollection<Kardex>? Kardex { get; set; }

    public ICollection<InventoryDetail>? InventoryDetails { get; set; }
}