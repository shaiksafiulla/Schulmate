using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SchoolCoreMOB.Helpers
{
    public class SessionCheckFilter 
    {
        private readonly RequestDelegate _next;

        public SessionCheckFilter(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the session has the key "UserLoggedIn"
            if (context.Session.GetString("UserInfo") == null)
            {
                // Redirect to login if the key does not exist
                context.Response.Redirect("/Login/Index", true);
                return;
            }

            // Proceed to the next middleware if the session key exists
            await _next(context);
        }
    }
}
