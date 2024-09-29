using System.ComponentModel.DataAnnotations;

namespace VendaPues.Shared.Entities;

public class ProductImage
{
    public int Id { get; set; }

    public Product? Product { get; set; }

    public int ProductId { get; set; }

    [Display(Name = "Imagén")]
    public string Image { get; set; } = null!;
}