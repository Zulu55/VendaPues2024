using System.ComponentModel.DataAnnotations;

namespace VendaPues.Shared.DTOs;

public class ImageDTO
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public List<string> Images { get; set; } = null!;
}