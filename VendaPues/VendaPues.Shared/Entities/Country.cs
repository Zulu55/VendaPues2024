using System.ComponentModel.DataAnnotations;
using VendaPues.Shared.Interfaces;

namespace VendaPues.Shared.Entities
{
    public class Country : IEntityWithName
    {
        public int Id { get; set; }

        [Display(Name = "País")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres.")]
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; } = null!;

        public ICollection<State>? States { get; set; }

        [Display(Name = "Departamentos / Estados")]
        public int StatesNumber => States == null || States.Count == 0 ? 0 : States.Count;
    }
}