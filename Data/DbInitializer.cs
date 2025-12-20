using FitnessCenterProject.Models;
using Microsoft.AspNetCore.Identity;

namespace FitnessCenterProject.Data
{
    public static class DbInitializer
    {
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // 1. ROLLERİ OLUŞTUR (Admin ve Üye)
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await roleManager.RoleExistsAsync("Member"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Member"));
                }

                // 2. ADMIN KULLANCISINI OLUŞTUR
                // NOT: Buradaki mail adresini kendi öğrenci numaranla güncelle!
                var adminEmail = "b221210082@sakarya.edu.tr";

                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    var newAdmin = new ApplicationUser()
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FullName = "Sistem Yöneticisi",
                        EmailConfirmed = true
                    };

                    // Şifre: sau (Program.cs'de kuralları gevşetmezsek hata verir)
                    var result = await userManager.CreateAsync(newAdmin, "sau");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                    }
                }
            }
        }
    }
}