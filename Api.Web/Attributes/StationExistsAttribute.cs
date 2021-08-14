using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Services.Services;
using Api.Web.Extensions;
using Api.Repository.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using ServiceStack.Redis;
using Api.Domain.Models;

namespace Api.Web.Attributes
{
    public class StationExistsAttribute : TypeFilterAttribute
    {
        public StationExistsAttribute() : base(typeof(StationExistsFilter)) {}

        private class StationExistsFilter : IAsyncActionFilter
        {
            private readonly IStationRepository _stationRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;
            private readonly IRedisClientsManagerAsync _redisManager;

            public StationExistsFilter
            (
                IStationRepository stationRepository,
                IStringLocalizer<SharedResources> localizer,
                IRedisClientsManagerAsync redisManager
            )
            {
                _stationRepository = stationRepository;
                _localizer = localizer;
                _redisManager = redisManager;
            }
            
            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var token = context.HttpContext.Request.Headers.ExtractJsonWebToken();

                    string role = token.SelectClaim("role");
                    string stationId = token.SelectClaim("station");

                    if (role != Roles.SuperAdmin && id != stationId)
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            Status = false,
                            Code = "InvalidOperation",
                            Message = _localizer["InvalidOperation"].Value
                        });
                        return;
                    }

                    var station = await _stationRepository.GetByIdAsync(id);

                    if (station is null) {
                        context.Result = new NotFoundObjectResult(new
                        {
                            Status = false,
                            Message = _localizer["StationNotFound"].Value,
                            Code = "StationNotFound"
                        });
                        return;
                    }

                    await using var redisClient = await _redisManager.GetClientAsync();
                    await redisClient.SetAsync<Station>
                        (
                            station.Id,
                            station,
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