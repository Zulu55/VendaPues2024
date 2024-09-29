using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;

namespace VendaPues.Backend.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersHelper _ordersHelper;
        private readonly IOrdersUnitOfWork _ordersUnitOfWork;

        public OrdersController(IOrdersHelper ordersHelper, IOrdersUnitOfWork ordersUnitOfWork)
        {
            _ordersHelper = ordersHelper;
            _ordersUnitOfWork = ordersUnitOfWork;
        }

        [HttpGet("recordsNumber")]
        public async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _ordersUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync(OrderDTO orderDTO)
        {
            var response = await _ordersUnitOfWork.UpdateFullAsync(User.Identity!.Name!, orderDTO);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest(response.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _ordersUnitOfWork.GetAsync(id);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return NotFound(response.Message);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _ordersUnitOfWork.GetAsync(User.Identity!.Name!, pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDTO pagination)
        {
            var action = await _ordersUnitOfWork.GetTotalPagesAsync(User.Identity!.Name!, pagination);
            if (action.WasSuccess)
            {
                return Ok(action.Result);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(OrderDTO orderDTO)
        {
            var response = await _ordersHelper.ProcessOrderAsync(User.Identity!.Name!, orderDTO);
            if (response.WasSuccess)
            {
                return NoContent();
            }

            return BadRequest(response.Message);
        }

        [HttpPost("report")]
        public async Task<IActionResult> PostAsync(DatesDTO datesDTO)
        {
            var response = await _ordersUnitOfWork.GetReportAsync(datesDTO);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }

            return BadRequest(response.Message);
        }
    }
}