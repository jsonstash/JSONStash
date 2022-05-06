using JSONStash.Common.Models;
using JSONStash.Web.Service.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JSONStash.Common.Context;

namespace JSONStash.Web.Service.Attributes
{
    public class KeyAuthorizeAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) 
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("x-api-key", out StringValues apiKey))
            {
                if (context.HttpContext.Request.RouteValues.TryGetValue("stashId", out object stashId))
                {
                    if (Guid.TryParse((string)stashId, out Guid stashGuid) && Guid.TryParse(apiKey, out Guid key))
                    {
                        JSONStashContext jsonStashContext = context.HttpContext.RequestServices.GetService<JSONStashContext>();

                        bool isValid = jsonStashContext.Stashes.Any(stash => stash.StashGuid.Equals(stashGuid) && stash.Key.Equals(key));

                        if (!isValid)
                            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    }
                    else
                    {
                        context.Result = new JsonResult(new { message = "Missing api key." }) { StatusCode = StatusCodes.Status400BadRequest };
                    }
                }
                else
                {
                    context.Result = new JsonResult(new { message = "Missing stash id." }) { StatusCode = StatusCodes.Status400BadRequest };
                }
            }
            else
            {
                context.Result = new JsonResult(new { message = "Missing api key." }) { StatusCode = StatusCodes.Status400BadRequest };
            }
        }
    }
}
