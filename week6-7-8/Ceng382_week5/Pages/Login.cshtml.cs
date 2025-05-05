using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Ceng382_week5.Models;
using Microsoft.AspNetCore.Http;
using Ceng382_week5.Data;
using Microsoft.EntityFrameworkCore;

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
 
        private readonly SchoolDbContext _context;
         public LoginModel(  SchoolDbContext context)
        { 
            _context = context;
        }
         
        public void OnGet()
        {
            if (Request.Cookies["remember_me"] == "true" && Request.Cookies["username"] != null)
            {
                Username = Request.Cookies["username"] ?? string.Empty;
                RememberMe = true;
            }
        }

       public async Task<IActionResult> OnPostAsync()
{
      try
    {
        var foundUser = await _context.Users
            .FirstOrDefaultAsync(u => 
                u.Username == Username && 
                u.Password == Password && 
                u.IsActive);

        if (foundUser == null)
        {
            ErrorMessage = "Geçersiz kullanıcı adı veya şifre!";
            return Page();
        }

        
 

        // Session işlemleri
        HttpContext.Session.SetString("username", foundUser.Username);
        HttpContext.Session.SetString("role", foundUser.Role);

        // Cookie ayarları
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // Development için
            SameSite = SameSiteMode.Lax
        };

        if (RememberMe)
        {
            cookieOptions.Expires = DateTimeOffset.Now.AddDays(30);
            Response.Cookies.Append("remember_me", "true", cookieOptions);
            Response.Cookies.Append("username", foundUser.Username, cookieOptions);
        }

        return RedirectToPage("/Index");
    }
    catch (Exception ex)
    {
        ErrorMessage = $"Sistem hatası: {ex.Message}";
        return Page();
    }
}
    }
}