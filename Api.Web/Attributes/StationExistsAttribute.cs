using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Services.Services;
using Api.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Attributes
{
    public class StationExistsAttribute : TypeFilterAttribute
    {
        public StationExistsAttribute() : base(typeof(StationExistsFilter)) {}

        private class StationExistsFilter : IAsyncActionFilter
        {
            private readonly IStationRepository _stationRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;

            public StationExistsFilter
            (
                IStationRepository stationRepository,
                IStringLocalizer<SharedResources> localizer
            )
                => (_stationRepository, _localizer) = (stationRepository, localizer);
            
            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var token = context.HttpContext.Request.Headers.ExtractJsonWebToken();
                    
                    var handler = new JwtSecurityTokenHandler();
                    var decoded = handler.ReadJwtToken(token);

                    string role = ((List<Claim>)decoded.Claims)?
                        .Where(claim => claim.Type == "role")
                        .Select(claim => claim.Value)
                        .SingleOrDefault();
                    
                    string stationId = ((List<Claim>)decoded.Claims)?
                        .Where(claim => claim.Type == "station")
                        .Select(claim => claim.Value)
                        .SingleOrDefault();

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