using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Identity Ayarlarý (ÞÝFRE KURALINI GEVÞETME)
//  PDF'te þifre "sau" istendiði için kurallarý kapatýyoruz.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false; // Rakam zorunlu deðil
    options.Password.RequireLowercase = false; // Küçük harf zorunlu deðil
    options.Password.RequireUppercase = false; // Büyük harf zorunlu deðil
    options.Password.RequireNonAlphanumeric = false; // Özel karakter (!,+,*) zorunlu deðil
    options.Password.RequiredLength = 3; // En az 3 karakter (normalde 6'dýr)
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. SEED DATA (Veritabanýný Doldur)
// Uygulama her baþladýðýnda Admin var mý diye kontrol eder.
await DbInitializer.Seed(app);

// Pipeline Ayarlarý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();