using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Extensions;
using Api.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Authorize]
    [Route("api/v1/customer-purchases")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class CustomerPurchaseController : ControllerBase
    {
        private readonly ICustomerPurchaseRepository _customerPurchaseRepository;

        public CustomerPurchaseController(ICustomerPurchaseRepository customerPurchaseRepository)
            => _customerPurchaseRepository = customerPurchaseRepository;

        #region snippet_Get

        [HttpGet]
        [ProducesResponseType(typeof(ListCustomerPurchaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin })]
        [SetPaginate]
        public async Task<IActionResult> GetAllAsync([FromQuery] ListResourceRequest request)
        {
            var totalDocuments = await _customerPurchaseRepository.CountAsync(request);
            var purchases = await _customerPurchaseRepository.GetAllAsync(request);
            return Ok(new
            {
                Status = true,
                Data = purchases,
                Paginator = Paginator.Paginate(request, totalDocuments)
            });
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ListCustomerPurchaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        [SetPaginate]
        public async Task<IActionResult> GetMeAsync([FromQuery] ListResourceRequest request)
        {
            var token = HttpContext.Request.Headers.ExtractJsonWebToken();
            var handler = new JwtSecurityTokenHandler();
            var decoded = handler.ReadJwtToken(token);

            string station = ((List<Claim>)decoded.Claims)?
                .Where(claim => claim.Type == "station")
                .Select(claim => claim.Value)
                .SingleOrDefault();

            request.Filters = string.IsNullOrEmpty(request.Filters) ?
                $"station={station}" :
                request.Filters + $",station={station}";

            var totalDocuments = await _customerPurchaseRepository.CountAsync(request);
            var purchases = await _customerPurchaseRepository.GetAllAsync(request);

            return Ok(new
            {
                Status = true,
                Data = purchases,
                Paginator = Paginator.Paginate(request, totalDocuments)
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SingleCustomerPurchaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        [CustomerPurchaseExists]
        public async Task<IActionResult> GetByIdAsync(string id, [FromQuery] SingleResourceRequest request)
        {
            var purchase = await _customerPurchaseRepository.GetByIdAsync(id);
            return Ok(new { Status = true, Data = purchase});
        }

        #endregion
    }
}