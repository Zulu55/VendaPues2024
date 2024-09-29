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
public class NewsController : GenericController<NewsArticle>
{
    private readonly INewsUnitOfWork _newsUnitOfWork;

    public NewsController(IGenericUnitOfWork<NewsArticle> unitOfWork, INewsUnitOfWork newsUnitOfWork) : base(unitOfWork)
    {
        _newsUnitOfWork = newsUnitOfWork;
    }

    [HttpPut]
    public override async Task<IActionResult> PutAsync(NewsArticle model)
    {
        var action = await _newsUnitOfWork.UpdateAsync(model);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [HttpPost]
    public override async Task<IActionResult> PostAsync(NewsArticle model)
    {
        var action = await _newsUnitOfWork.AddAsync(model);
        if (action.WasSuccess)
        {
            return Ok(action.Result);
        }
        return BadRequest(action.Message);
    }

    [AllowAnonymous]
    [HttpGet("recordsNumber")]
    public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _newsUnitOfWork.GetRecordsNumberAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }

    [AllowAnonymous]
    [HttpGet]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _newsUnitOfWork.GetAsync(pagination);
        if (response.WasSuccess)
        {
            return Ok(response.Result);
        }
        return BadRequest();
    }
}