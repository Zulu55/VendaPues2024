using VendaPues.Shared.DTOs;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Helpers;

public interface IPurchaseHelper
{
    Task<ActionResponse<bool>> ProcessPurchaseAsync(PurchaseDTO purchaseDTO, string email);
}