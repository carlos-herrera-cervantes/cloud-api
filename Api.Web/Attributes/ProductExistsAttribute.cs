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
    public class ProductExistsAttribute : TypeFilterAttribute
    {
        public ProductExistsAttribute() : base(typeof(ProductExistsFilter)) {}

        private class ProductExistsFilter : IAsyncActionFilter
        {
            private readonly IProductRepository _productRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;
            private readonly IRedisClientsManagerAsync _redisManager;

            public ProductExistsFilter
            (
                IProductRepository productRepository,
                IStringLocalizer<SharedResources> localizer,
                IRedisClientsManagerAsync redisManager
            )
            {
                _productRepository = productRepository;
                _localizer = localizer;
                _redisManager = redisManager;
            }

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

                    await using var redisClient = await _redisManager.GetClientAsync();
                    await redisClient.SetAsync<Product>
                        (
                            product.Id,
                            product,
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
        }
    }
}