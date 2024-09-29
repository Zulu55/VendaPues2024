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
    public class PurchaseDetailsController : GenericController<PurchaseDetail>
    {
        private readonly IPurchaseDetailUnitOfWork _purchaseDetailUnitOfWork;

        public PurchaseDetailsController(IGenericUnitOfWork<PurchaseDetail> unitOfWork, IPurchaseDetailUnitOfWork purchaseDetailUnitOfWork) : base(unitOfWork)
        {
            _purchaseDetailUnitOfWork = purchaseDetailUnitOfWork;
        }

        [HttpGet("recordsNumber")]
        public override async Task<IActionResult> GetRecordsNumberAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _purchaseDetailUnitOfWork.GetRecordsNumberAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }

        [HttpGet]
        public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
        {
            var response = await _purchaseDetailUnitOfWork.GetAsync(pagination);
            if (response.WasSuccess)
            {
                return Ok(response.Result);
            }
            return BadRequest();
        }
    }
}