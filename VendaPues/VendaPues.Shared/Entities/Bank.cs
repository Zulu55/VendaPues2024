using System.ComponentModel.DataAnnotations;
using VendaPues.Shared.Interfaces;

namespace VendaPues.Shared.Entities
{
    public class Bank : IEntityWithName
    {
        public int Id { get; set; }

        [Display(Name = "Banco")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = null!;

        public ICollection<OrderPayment>? OrderPayments { get; set; }
    }
}