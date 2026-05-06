using MedGyn.MedForce.Common.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MedGyn.MedForce.Web.Middleware
{
    public class AuthorizeFilter : IActionFilter
    {
        public AuthorizeFilter()
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //this filter will run on any MVC controller that the filter has been added to, and will check the logged in state
            // of the user. It can be improved by implementing something like IdentityServer since Session is not the most safe way to
            // store this data, but due to time constraints was the workaround.
            if (context.HttpContext.Session.GetInt32(SessionConstants.UserIdKey) == null)
            {
                //User has not logged in, send them to login page
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path.Value });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
