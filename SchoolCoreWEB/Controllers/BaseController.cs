using Microsoft.AspNetCore.Mvc;


namespace SchoolCoreWEB.Controllers
{
    public class BaseController : Controller
    {
        // Common check function to verify session key existence
        protected IActionResult CheckSessionKey(string sessionKey, IActionResult redirectTo)
        {
            if (!HttpContext.Session.Keys.Contains(sessionKey))
            {
                // Redirect to login page or another appropriate page
                return redirectTo;
            }
            return null; // Continue with the action execution
        }
    }
}
