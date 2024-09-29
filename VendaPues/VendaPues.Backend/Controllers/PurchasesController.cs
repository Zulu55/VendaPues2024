using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class PurchasesController : GenericController<Purchase>
{
    private readonly IPurchaseUnitOfWork _purchaseUnitOfWork;
    private readonly IPurchaseHelper _purchaseHelper;

    public PurchasesController(IGenericUnitOfWork<Purchase> unitOfWork, IPurchaseUnitOfWork purchaseUnitOfWork, IPurchaseHelper purchaseHelper) : base(unitOfWork)
    {
        _purchaseUnitOfWork = purchaseUnitOfWork;
        _purchaseHelper = purchaseHelper;
    }

    [HttpGet("recordsNumber")]
    public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _purchaseUnitOfWork.GetRecordsNumberAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _purchaseUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var action = await _purchaseUnitOfWork.GetAsync(id);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return NotFound(action.Message);
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync(PurchaseDTO purchaseDTO)
    {
        var response = await _purchaseHelper.ProcessPurchaseAsync(purchaseDTO, User.Identity!.Name!);
        if (!response.WasSuccess)
        {
            return NoContent();
        }

        return BadRequest(response.Message);
    }
}