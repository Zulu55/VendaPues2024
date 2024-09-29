using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class TemporalPurchasesController : GenericController<TemporalPurchase>
{
    private readonly ITemporalPurchasesUnitOfWork _temporalPurchasesUnitOfWork;

    public TemporalPurchasesController(IGenericUnitOfWork<TemporalPurchase> unitOfWork, ITemporalPurchasesUnitOfWork temporalPurchasesUnitOfWork) : base(unitOfWork)
    {
        _temporalPurchasesUnitOfWork = temporalPurchasesUnitOfWork;
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _temporalPurchasesUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return NotFound(response.Message);
    }

    [HttpPut("full")]
    public async Task<IActionResult> PutFullAsync(TemporalPurchaseDTO temporalPurchaseDTO)
    {
        var action = await _temporalPurchasesUnitOfWork.PutFullAsync(temporalPurchaseDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return NotFound(action.Message);
    }

    [HttpPost("full")]
    public async Task<IActionResult> PostAsync(TemporalPurchaseDTO temporalPurchaseDTO)
    {
        var action = await _temporalPurchasesUnitOfWork.AddFullAsync(User.Identity!.Name!, temporalPurchaseDTO);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpGet("my")]
    public override async Task<IActionResult> GetAsync()
    {
        var action = await _temporalPurchasesUnitOfWork.GetAsync(User.Identity!.Name!);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCountAsync()
    {
        var action = await _temporalPurchasesUnitOfWork.GetCountAsync(User.Identity!.Name!);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }
}