using System;
using System.Threading.Tasks;
using Api.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Api.Web.Middlewares
{
    public class StationExistsAttribute : TypeFilterAttribute
    {
        public StationExistsAttribute() : base(typeof(StationExistsFilter)) {}

        private class StationExistsFilter : IAsyncActionFilter
        {
            private readonly IStationRepository _stationRepository;
            private readonly IStringLocalizer<SharedResources> _localizer;

            public StationExistsFilter(IStationRepository stationRepository, IStringLocalizer<SharedResources> localizer)
                => (_stationRepository, _localizer) = (stationRepository, localizer);
            
            #region snippet_BeforeExecute

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                try
                {
                    var id = context.ActionArguments["id"] as string;
                    var station = await _stationRepository.GetByIdAsync(id);

                    if (station is null) {
                        context.Result = new NotFoundObjectResult(new { Status = false, Message = _localizer["StationNotFound"].Value, Code = "StationNotFound" });
                        return;
                    }

                    await next();
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