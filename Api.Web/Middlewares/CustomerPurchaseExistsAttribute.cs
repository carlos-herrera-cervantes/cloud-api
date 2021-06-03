using System;
using System.Threading.Tasks;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Middlewares
{
    public class CustomerPurchaseExistsAttribute : TypeFilterAttribute
    {
        public CustomerPurchaseExistsAttribute() : base(typeof(CustomerPurchaseExistsFilter)) {}

        private class CustomerPurchaseExistsFilter : IAsyncActionFilter
        {
            private readonly ICustomerPurchaseRepository _customerPurchaseRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;

            public CustomerPurchaseExistsFilter
            (
                ICustomerPurchaseRepository customerPurchaseRepository,
                IStringLocalizer<SharedResources> localizer
            )
                => (_customerPurchaseRepository, _localizer) = (customerPurchaseRepository, localizer);
            
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