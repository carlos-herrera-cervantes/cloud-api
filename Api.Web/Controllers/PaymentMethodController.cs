using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Common;
using Api.Web.Handlers;
using Api.Web.Attributes;
using Api.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Authorize]
    [Route("api/v1/payment-methods")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly string _collection = "payments";
        private readonly IPaymentMethodManager _paymentMethodManager;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOperationHandler _operationHandler;

        public PaymentMethodController(
            IPaymentMethodManager paymentMethodManager, 
            IPaymentMethodRepository paymentMethodRepository, 
            IOperationHandler operationHandler
        )
        => (_paymentMethodManager, _paymentMethodRepository, _operationHandler) = (paymentMethodManager, paymentMethodRepository, operationHandler);

        #region snippet_GetAll

        [HttpGet]
        [ProducesResponseType(typeof(ListPaymentMethodResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        [SetPaginate]
        public async Task<IActionResult> GetAllAsync([FromQuery] ListResourceRequest request)
        {
            var totalDocuments = await _paymentMethodRepository.CountAsync(request);
            var paymentMethods = await _paymentMethodRepository.GetAllAsync(request);
            return Ok(new { Status = true, Data = paymentMethods, Paginator = Paginator.Paginate(request, totalDocuments) });
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SinglePaymentMethodResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        [PaymentMethodExists]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            return Ok(new { Status = true, Data = paymentMethod });
        }

        #endregion

        #region snipppet_Create

        [HttpPost]
        [ProducesResponseType(typeof(SinglePaymentMethodResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        public async Task<IActionResult> CreateAsync(PaymentMethod paymentMethod)
        {
            await _paymentMethodManager.CreateAsync(paymentMethod);
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Create,
                Collection = _collection,
                Id = paymentMethod.Id,
                Model = paymentMethod
            });
            
            return Created("", new { Status = true, Data = paymentMethod });
        }

        #endregion

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(SinglePaymentMethodResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        [PaymentMethodExists]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<PaymentMethod> replacePaymentMethod)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            await _paymentMethodManager.UpdateByIdAsync(id, paymentMethod, replacePaymentMethod);
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Update,
                Collection = _collection,
                Id = id,
                Model = paymentMethod
            });

            return Created("", new { Status = true, Data = paymentMethod });
        }

        #endregion

        #region snippet_Delete

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Role(roles: new [] { Roles.SuperAdmin, Roles.StationAdmin })]
        [PaymentMethodExists]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _paymentMethodManager.DeleteByIdAsync(id);
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Delete,
                Collection = _collection,
                Id = id,
                Model = null
            });

            return NoContent();
        }

        #endregion
    }
}