using System;
using System.Threading.Tasks;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Middlewares
{
    public class PaymentMethodExistsAttribute : TypeFilterAttribute
    {
        public PaymentMethodExistsAttribute() : base(typeof(PaymentMethodExistsFilter)) { }

        private class PaymentMethodExistsFilter : IAsyncActionFilter
        {
            private readonly IPaymentMethodRepository _paymentMethodRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;

            public PaymentMethodExistsFilter(IPaymentMethodRepository paymentMethodRepository, IStringLocalizer<SharedResources> localizer)
                => (_paymentMethodRepository, _localizer) = (paymentMethodRepository, localizer);

            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);

                    if (paymentMethod is null) {
                        context.Result = new NotFoundObjectResult(new { Status = false, Message = _localizer["PaymentNotFound"].Value, Code = "PaymentNotFound" });
                        return;
                    }
                }
                catch (FormatException)
                {
                    context.Result = new BadRequestObjectResult(new { Status = false, Message = _localizer["InvalidObjectId"].Value, Code = "InvalidObjectId" });
                    return;
                }
            }

            #endregion
        }
    }
}