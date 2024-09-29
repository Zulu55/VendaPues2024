namespace VendaPues.Shared.DTOs;

public class PaymentDTO
{
    public int BankId { get; set; }

    public string Email { get; set; } = null!;

    public decimal Value { get; set; }
}