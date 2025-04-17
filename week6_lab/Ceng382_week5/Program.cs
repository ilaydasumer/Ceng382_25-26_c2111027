var builder = WebApplication.CreateBuilder(args);

// Razor Pages ve Session servisi ekleniyor
builder.Services.AddRazorPages();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "MyApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Giriş yapıldıktan sonra 30 dakika aktif kalsın
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// GDPR için çerez politikası ayarı
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCookiePolicy();  // Çerez politikası uygulanıyor
app.UseRouting();

app.UseSession();       // Session burada çağrılıyor (routing'den sonra, auth'dan önce)
app.UseAuthorization();

app.MapRazorPages();

// 🔥 Sadece bir tane yönlendirme bırakıyoruz
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.Run();
