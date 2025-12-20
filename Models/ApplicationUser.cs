using Microsoft.AspNetCore.Identity;

namespace FitnessCenterProject.Models
{
    // IdentityUser'dan miras alarak standart özelliklere (email, password) sahip oluyoruz.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } // Ad Soyad

        // AI Diyet/Egzersiz önerisi için gerekli alanlar
        public int? Height { get; set; } // Boy (cm)
        public int? Weight { get; set; } // Kilo (kg)
        public string? BodyType { get; set; } // Vücut Tipi (Opsiyonel)
        public string? ProfilePhotoPath { get; set; } // Profil Resmi Yolu
    }
}