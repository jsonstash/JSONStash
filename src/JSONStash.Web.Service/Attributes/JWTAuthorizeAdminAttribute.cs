using JSONStash.Common.Enums;
using JSONStash.Common.Models;
using JSONStash.Web.Service.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JSONStash.Web.Service.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JWTAuthorizeAdminAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool exists = context.HttpContext.TryParseItem("User", out User user);
            if (!exists && !user.Role.Equals(RoleType.Administrator))
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
