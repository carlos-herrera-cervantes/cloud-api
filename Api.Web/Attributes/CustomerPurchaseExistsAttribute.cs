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
    public class CustomerPurchaseExistsAttribute : TypeFilterAttribute
    {
        public CustomerPurchaseExistsAttribute() : base(typeof(CustomerPurchaseExistsFilter)) {}

        private class CustomerPurchaseExistsFilter : IAsyncActionFilter
        {
            private readonly ICustomerPurchaseRepository _customerPurchaseRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;
            private readonly IRedisClientsManagerAsync _redisManager;

            public CustomerPurchaseExistsFilter
            (
                ICustomerPurchaseRepository customerPurchaseRepository,
                IStringLocalizer<SharedResources> localizer,
                IRedisClientsManagerAsync redisManager
            )
            {
                _customerPurchaseRepository = customerPurchaseRepository;
                _localizer = localizer;
                _redisManager = redisManager;
            }
            
            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var purchase = await _customerPurchaseRepository.GetByIdAsync(id);

                    if (purchase is null)
                    {
                        context.Result = new NotFoundObjectResult(new
                        {
                            Status = false,
                            Message = _localizer["PurchaseNotFound"].Value,
                            Code = "PurchaseNotFound"
                        });
                        return ;
                    }

                    await using var redisClient = await _redisManager.GetCacheClientAsync();
                    await redisClient.SetAsync<CustomerPurchase>
                        (
                            purchase.Id,
                            purchase,
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