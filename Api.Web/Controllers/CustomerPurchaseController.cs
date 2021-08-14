using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Repository.Extensions;
using Api.Services.Services;
using Api.Web.Extensions;
using Api.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;

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
        private readonly IRedisClientsManagerAsync _redisManager;

        public CustomerPurchaseController
        (
            ICustomerPurchaseRepository customerPurchaseRepository,
            IRedisClientsManagerAsync redisManager
        )        
        {
            _customerPurchaseRepository = customerPurchaseRepository;
            _redisManager = redisManager;
        }

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

            return Ok(new ListCustomerPurchaseResponse
            {
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
            var station = token.SelectClaim("station");

            request.Filters = string.IsNullOrEmpty(request.Filters) ?
                $"station={station}" :
                request.Filters + $",station={station}";

            var totalDocuments = await _customerPurchaseRepository.CountAsync(request);
            var purchases = await _customerPurchaseRepository.GetAllAsync(request);

            return Ok(new ListCustomerPurchaseResponse
            {
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
            await using var redisClient = await _redisManager.GetClientAsync();
            var purchase = await redisClient.GetAsync<CustomerPurchase>(id) ??
                await _customerPurchaseRepository.GetByIdAsync(id);

            return Ok(new SingleCustomerPurchaseResponse { Data = purchase });
        }

        #endregion
    }
}