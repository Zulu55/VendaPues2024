using System.ComponentModel.DataAnnotations;

namespace VendaPues.Shared.Entities
{
    public class Inventory
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime Date { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Description { get; set; } = null!;

        public string FullName => $"{Name}, {Date.ToLocalTime():yyyy/MM/dd hh:mm tt}"; 

        [Display(Name = "Conteo 1 finalizado")]
        public bool Count1Finish { get; set; }

        [Display(Name = "Conteo 2 finalizado")]
        public bool Count2Finish { get; set; }

        [Display(Name = "Conteo 3 finalizado")]
        public bool Count3Finish { get; set; }

        public ICollection<InventoryDetail>? InventoryDetails { get; set; }

    }
}