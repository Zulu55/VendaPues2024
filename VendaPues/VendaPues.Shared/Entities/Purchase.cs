using System.ComponentModel.DataAnnotations;

namespace VendaPues.Shared.Entities
{
    public class Purchase
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public DateTime Date { get; set; }

        public Supplier? Supplier { get; set; }

        public int SupplierId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string? Remarks { get; set; }

        public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Líneas")]
        public int Lines => PurchaseDetails == null || PurchaseDetails.Count == 0 ? 0 : PurchaseDetails.Count;

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Cantidad")]
        public float Quantity => PurchaseDetails == null || PurchaseDetails.Count == 0 ? 0 : PurchaseDetails.Sum(sd => sd.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Valor")]
        public decimal Value => PurchaseDetails == null || PurchaseDetails.Count == 0 ? 0 : PurchaseDetails.Sum(sd => sd.Value);
    }
}