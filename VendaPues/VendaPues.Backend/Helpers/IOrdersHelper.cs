using VendaPues.Shared.DTOs;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Helpers;

public interface IOrdersHelper
{
    Task<ActionResponse<bool>> ProcessOrderAsync(string email, OrderDTO orderDTO);
}