using System;
using System.Threading.Tasks;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Middlewares
{
    public class ProductExistsAttribute : TypeFilterAttribute
    {
        public ProductExistsAttribute() : base(typeof(ProductExistsFilter)) {}

        private class ProductExistsFilter : IAsyncActionFilter
        {
            private readonly IProductRepository _productRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;

            public ProductExistsFilter
            (
                IProductRepository productRepository,
                IStringLocalizer<SharedResources> localizer
            )
                => (_productRepository, _localizer) = (productRepository, localizer);

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var product = await _productRepository.GetByIdAsync(id);

                    if (product is null)
                    {
                        context.Result = new NotFoundObjectResult(new
                        {
                            Status = false,
                            Message = _localizer["ProductNotFound"].Value,
                            Code = "ProductNotFound"
                        });
                        return;
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
        }
    }
}