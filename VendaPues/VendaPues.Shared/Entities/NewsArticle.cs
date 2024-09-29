using System.ComponentModel.DataAnnotations;

namespace VendaPues.Shared.Entities
{
    public class NewsArticle
    {
        public int Id { get; set; }

        [Display(Name = "Título")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Title { get; set; } = null!;

        [Display(Name = "Resumen")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Summary { get; set; } = null!;

        [Display(Name = "Resumen")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string ImageUrl { get; set; } = null!;

        public bool Active { get; set; }
    }
}