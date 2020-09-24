using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Authorize]
    [Route("api/v1/payment-methods")]
    [Produces("application/json")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodManager _paymentMethodManager;

        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public PaymentMethodController(IPaymentMethodManager paymentMethodManager, IPaymentMethodRepository paymentMethodRepository)
            => (_paymentMethodManager, _paymentMethodRepository) = (paymentMethodManager, paymentMethodRepository);

        #region snippet_GetAll

        [HttpGet]
        [ProducesResponseType(typeof(PaymentMethod), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            var paymentMethods = await _paymentMethodRepository.GetAllAsync();
            return Ok(new { Status = true, Data = paymentMethods });
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaymentMethod), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [PaymentMethodExists]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            return Ok(new { Status = true, Data = paymentMethod });
        }

        #endregion

        #region snipppet_Create

        [HttpPost]
        [ProducesResponseType(typeof(PaymentMethod), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync(PaymentMethod paymentMethod)
        {
            await _paymentMethodManager.CreateAsync(paymentMethod);
            return Created("", new { Status = true, Data = paymentMethod });
        }

        #endregion

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(PaymentMethod), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [PaymentMethodExists]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<PaymentMethod> replacePaymentMethod)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            await _paymentMethodManager.UpdateByIdAsync(id, paymentMethod, replacePaymentMethod);
            return Created("", new { Status = true, Data = paymentMethod });
        }

        #endregion

        #region snippet_Delete

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [PaymentMethodExists]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _paymentMethodManager.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion
    }
}