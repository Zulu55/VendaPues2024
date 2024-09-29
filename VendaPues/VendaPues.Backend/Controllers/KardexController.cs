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
public class KardexController : GenericController<Kardex>
{
    private readonly IKardexUnitOfWork _kardexUnitOfWork;

    public KardexController(IGenericUnitOfWork<Kardex> unitOfWork, IKardexUnitOfWork kardexUnitOfWork) : base(unitOfWork)
    {
        _kardexUnitOfWork = kardexUnitOfWork;
    }

    [HttpGet("recordsNumber")]
    public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _kardexUnitOfWork.GetRecordsNumberAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _kardexUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }
}