using System.Text.Json;
using Ceng382_week5.Data;
using Ceng382_week5.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages ve Session servisi ekleniyor
builder.Services.AddRazorPages()
    .AddNewtonsoftJson();

// DbContext yapılandırması
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SchoolDbConnection")));
 
 

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "MyApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
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
builder.Services.AddRazorPages();
var app = builder.Build();
app.UseCookiePolicy();
app.UseSession(); // UseRouting'den önce olmalı
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if (!path.StartsWithSegments("/Login") && 
        !path.StartsWithSegments("/Error") &&
        context.Session.GetString("username") == null)
    {
        context.Response.Redirect("/Login");
        return;
    }
    await next();
});

 

// I got help from AI for creating the database and adding sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
    
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
 
    if (!context.Users.Any())
    {
        Console.WriteLine("Kullanıcılar tablosu boş, veriler yükleniyor...");
        
       var filePath = Path.Combine(env.ContentRootPath, "Models", "data", "users.json");
        Console.WriteLine($"Aranan dosya yolu: {filePath}");
        if (System.IO.File.Exists(filePath))
        {
            var jsonData = await System.IO.File.ReadAllTextAsync(filePath);
            Console.WriteLine("JSON dosyası okundu:");
            Console.WriteLine(jsonData);

            var users = JsonSerializer.Deserialize<List<User>>(jsonData);
            
            if (users != null && users.Any())
            {
                Console.WriteLine($"{users.Count} kullanıcı bulundu, ekleniyor...");
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
                Console.WriteLine("Kullanıcılar başarıyla eklendi.");
            }
            else
            {
                Console.WriteLine("JSON dosyasında kullanıcı bulunamadı.");
            }
        }
        else
        {
            Console.WriteLine("users.json dosyası bulunamadı!");
        }
    }

    if (!context.Classes.Any())
    {
 
    context.Database.ExecuteSqlRaw("TRUNCATE TABLE Classes");
    context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Classes', RESEED, 0)");
 
    var subjects = new[] { "Mathematics", "Physics", "Chemistry", "Biology", "Programming" };
    var rnd = new Random();
    
    for (int i = 1; i <= 100; i++)
    {
        context.Classes.Add(new ClassInformationModel
        { 
            ClassName = $"{subjects[i % subjects.Length]}",
            StudentCount = rnd.Next(15, 50),
            Description = $"Course description for {subjects[i % subjects.Length]} {i}",
            IsActive = true,
 
        });

        if (i % 20 == 0) await context.SaveChangesAsync();
    }
    await context.SaveChangesAsync();
    
 
    var firstRecord = await context.Classes.OrderBy(c => c.Id).FirstOrDefaultAsync();
    Console.WriteLine($"First record ID: {firstRecord?.Id}"); // 1 olmalı




}
}
app.MapRazorPages();
app.Run();