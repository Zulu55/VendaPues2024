using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class StatesController : GenericController<State>
    {
        private readonly IStatesUnitOfWork _statesUnitOfWork;

        public StatesController(IGenericUnitOfWork<State> unitOfWork, IStatesUnitOfWork statesUnitOfWork) : base(unitOfWork)
        {
            _statesUnitOfWork = statesUnitOfWork;
        }

        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _statesUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("combo/{countryId:int}")]
        public async Task<IActionResult> GetComboAsync(int countryId)
        {
            return Ok(await _statesUnitOfWork.GetComboAsync(countryId));
        }

        [HttpGet("full")]
        public override async Task<IActionResult> GetAsync()
        {
            var response = await _statesUnitOfWork.GetAsync();
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _statesUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public override async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _statesUnitOfWork.GetTotalPagesAsync(pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public override async Task<IActionResult> GetAsync(int id)
        {
            var response = await _statesUnitOfWork.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return NotFound(response.Message);
        }
    }
}
