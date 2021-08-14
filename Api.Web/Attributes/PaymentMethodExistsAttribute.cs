using System;
using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using ServiceStack.Redis;

namespace Api.Web.Attributes
{
    public class PaymentMethodExistsAttribute : TypeFilterAttribute
    {
        public PaymentMethodExistsAttribute() : base(typeof(PaymentMethodExistsFilter)) { }

        private class PaymentMethodExistsFilter : IAsyncActionFilter
        {
            private readonly IPaymentMethodRepository _paymentMethodRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;
            private readonly IRedisClientsManagerAsync _redisManager;

            public PaymentMethodExistsFilter
            (
                IPaymentMethodRepository paymentMethodRepository,
                IStringLocalizer<SharedResources> localizer,
                IRedisClientsManagerAsync redisManager
            )
            {
                _paymentMethodRepository = paymentMethodRepository;
                _localizer = localizer;
                _redisManager = redisManager;
            }

            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);

                    if (paymentMethod is null)
                    {
                        context.Result = new NotFoundObjectResult(new
                        {
                            Status = false,
                            Message = _localizer["PaymentNotFound"].Value,
                            Code = "PaymentNotFound"
                        });
                        return;
                    }

                    await using var redisClient = await _redisManager.GetClientAsync();
                    await redisClient.SetAsync<PaymentMethod>
                        (
                            paymentMethod.Id,
                            paymentMethod,
                            expiresAt: DateTime.UtcNow.AddMinutes(10)
                        );

                    await next();
                }
                catch (FormatException)
                {
                    context.Result = new BadRequestObjectResult(new
                    {
                        Status = false,
                        Message = _localizer["InvalidObjectId"].Value,
                        Code = "InvalidObjectId"
                    });
                    return;
                }
            }

            #endregion
        }
    }
}