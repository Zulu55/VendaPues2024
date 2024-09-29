using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Responses;

namespace VendaPues.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class PaymentsController : Controller
{
    private readonly Random random = new();

    [HttpPost]
    public async Task<IActionResult> PostAsync(PaymentDTO paymentDTO)
    {
        await Task.Delay(3000);
        var number = random.Next(100);
        if (number < 70)
        {
            return Ok(new ActionResponse<string>
            {
                WasSuccess = true,
                Result = Guid.NewGuid().ToString(),
                Message = "Transacción aprobada."
            });
        }
        return Ok(new ActionResponse<string>
        {
            WasSuccess = false,
            Message = "Transacción no aprobada."
        });
    }
}