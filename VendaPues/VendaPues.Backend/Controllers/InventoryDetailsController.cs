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
public class InventoryDetailsController : GenericController<InventoryDetail>
{
    private readonly IInventoryDetailsUnitOfWork _inventoryDetailsUnitOfWork;

    public InventoryDetailsController(IGenericUnitOfWork<InventoryDetail> unitOfWork, IInventoryDetailsUnitOfWork inventoryDetailsUnitOfWork) : base(unitOfWork)
    {
        _inventoryDetailsUnitOfWork = inventoryDetailsUnitOfWork;
    }

    [HttpPut]
    public override async Task<IActionResult> PutAsync(InventoryDetail model)
    {
        var response = await _inventoryDetailsUnitOfWork.UpdateAsync(model);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("recordsNumberCount1")]
    public async Task<IActionResult> GetRecordsNumberCount1Async([FromQuery] PaginationDTO pagination)
    {
        var response = await _inventoryDetailsUnitOfWork.GetRecordsNumberCount1Async(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("recordsNumberCount2")]
    public async Task<IActionResult> GetRecordsNumberCount2Async([FromQuery] PaginationDTO pagination)
    {
        var response = await _inventoryDetailsUnitOfWork.GetRecordsNumberCount2Async(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("recordsNumberCount3")]
    public async Task<IActionResult> GetRecordsNumberCount3Async([FromQuery] PaginationDTO pagination)
    {
        var response = await _inventoryDetailsUnitOfWork.GetRecordsNumberCount3Async(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("Count1")]
    public async Task<IActionResult> GetCount1Async([FromQuery] PaginationDTO pagination)
    {
        var response = await _inventoryDetailsUnitOfWork.GetCount1Async(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("Count2")]
    public async Task<IActionResult> GetCount2Async([FromQuery] PaginationDTO pagination)
    {
        var response = await _inventoryDetailsUnitOfWork.GetCount2Async(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("Count3")]
    public async Task<IActionResult> GetCount3Async([FromQuery] PaginationDTO pagination)
    {
        var response = await _inventoryDetailsUnitOfWork.GetCount3Async(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }
}