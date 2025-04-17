using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Ceng382_week5.Models;
using Microsoft.AspNetCore.Http;

namespace Ceng382_week5.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        private readonly IWebHostEnvironment _env;

        public LoginModel(IWebHostEnvironment env)
        {
            _env = env;
        }


// I got help for this part from AI.
        public void OnGet()
        { 
            if (Request.Cookies["remember_me"] == "true" && Request.Cookies["saved_username"] != null)
            {
                Username = Request.Cookies["saved_username"];
                RememberMe = true;
                 
                var authToken = Request.Cookies["auth_token"];
                if (!string.IsNullOrEmpty(authToken))
                {
                    HttpContext.Session.SetString("username", Request.Cookies["saved_username"]);
                    HttpContext.Session.SetString("token", authToken);
                    Response.Redirect("/Index");
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var filePath = Path.Combine(_env.WebRootPath, "data", "users.json");
                var jsonData = await System.IO.File.ReadAllTextAsync(filePath);
                var users = JsonSerializer.Deserialize<List<User>>(jsonData);

                var user = users?.FirstOrDefault(u => 
                    u.Username == Username && 
                    u.Password == Password && 
                    u.IsActive);

                if (user == null)
                {
                    ErrorMessage = "Invalid username or password";
                    return Page();
                }

 
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = RememberMe ? DateTimeOffset.Now.AddDays(30) : null,
                    IsEssential = true  
                };

                

                if (RememberMe)
                {
                    cookieOptions.Expires = DateTimeOffset.Now.AddDays(30);
                    Response.Cookies.Append("remember_me", "true", cookieOptions);
                    Response.Cookies.Append("saved_username", user.Username, cookieOptions);
                    Response.Cookies.Append("auth_token", token, cookieOptions);
                }
                else
                {
           
                    cookieOptions.Expires = DateTimeOffset.Now.AddMinutes(30);
                    Response.Cookies.Append("auth_token", token, cookieOptions);
                    
                Response.Cookies.Delete("remember_me", new CookieOptions { Path = "/" });
                Response.Cookies.Delete("saved_username", new CookieOptions { Path = "/" });
                                }

                Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = "System error: " + ex.Message;
                return Page();
            }
        }
    }
}