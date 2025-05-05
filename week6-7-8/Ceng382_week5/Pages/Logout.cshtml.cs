using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ceng382_week5.Pages
{
    public class LogoutModel : PageModel
    {
        
        public IActionResult OnGet()
        {
             
            HttpContext.Session.Clear();
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(-1),
                HttpOnly = true,
                Secure = true,
                Path = "/"
            };
            
            Response.Cookies.Append("auth_token", "", cookieOptions);
            Response.Cookies.Append("session_id", "", cookieOptions);
            Response.Cookies.Append("remember_me", "", cookieOptions);
            Response.Cookies.Append("saved_username", "", cookieOptions);
                    
                    
            

            
            return LocalRedirect("/Login");
        }
    }
}