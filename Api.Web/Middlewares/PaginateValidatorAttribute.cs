using System.Threading.Tasks;
using Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Web.Middlewares
{
  public class PaginateValidatorAttribute : TypeFilterAttribute
  {
    public PaginateValidatorAttribute() : base(typeof(PaginateValidatorFilter)) { }

    private class PaginateValidatorFilter : IAsyncActionFilter
    {
      public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
      {
        var (_, pageSize, page, paginate, _, _) = context.ActionArguments["querys"] as Request;
        var isValidPagination = page < 1 && pageSize > 0 && pageSize < 11;

        if (paginate && isValidPagination)
        {
          context.Result = new BadRequestObjectResult(new { Status = false, Code = "InvalidPagination" });
          return;
        }

        await next();
      }
    }
  }
}