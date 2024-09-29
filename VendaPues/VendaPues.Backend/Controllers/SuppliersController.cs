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
public class SuppliersController : GenericController<Supplier>
{
    private readonly ISuppliersUnitOfWork _suppliersUnitOfWork;

    public SuppliersController(IGenericUnitOfWork<Supplier> unitOfWork, ISuppliersUnitOfWork suppliersUnitOfWork) : base(unitOfWork)
    {
        _suppliersUnitOfWork = suppliersUnitOfWork;
    }

    [HttpGet("recordsNumber")]
    public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _suppliersUnitOfWork.GetRecordsNumberAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("combo")]
    public async Task<IActionResult> GetComboAsync()
    {
        return Ok(await _suppliersUnitOfWork.GetComboAsync());
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _suppliersUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet("one/{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var response = await _suppliersUnitOfWork.GetAsync(id);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }
}