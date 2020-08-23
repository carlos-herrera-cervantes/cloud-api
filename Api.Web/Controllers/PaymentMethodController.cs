using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Route("api/v1/payment-methods")]
    [ApiController]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodManager _paymentMethodManager;

        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public PaymentMethodController(IPaymentMethodManager paymentMethodManager, IPaymentMethodRepository paymentMethodRepository)
            => (_paymentMethodManager, _paymentMethodRepository) = (paymentMethodManager, paymentMethodRepository);

        /// <summary>
        /// GET
        /// </summay>

        #region snippet_GetAll

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var paymentMethods = await _paymentMethodRepository.GetAllAsync();
            return Ok(paymentMethods);
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            return Ok(paymentMethod);
        }

        #endregion

        /// <summary>
        /// POST
        /// </summary>

        #region snipppet_Create

        [HttpPost]
        public async Task<IActionResult> CreateAsync(PaymentMethod paymentMethod)
        {
            await _paymentMethodManager.CreateAsync(paymentMethod);
            return Created("", paymentMethod);
        }

        #endregion

        /// <summay>
        /// PATCH
        /// </summary>

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<PaymentMethod> replacePaymentMethod)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            await _paymentMethodManager.UpdateByIdAsync(id, paymentMethod, replacePaymentMethod);
            return Created("", paymentMethod);
        }

        #endregion

        /// <summary>
        /// DELETE
        /// </summary>

        #region snippet_Delete

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _paymentMethodManager.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion
    }
}