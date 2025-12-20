using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Antrenör adı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Display(Name = "Uzmanlık Alanı")]
        public string Specialty { get; set; } // Örn: Kilo Alma, Vücut Geliştirme

        // Basitlik adına çalışma saatlerini metin olarak tutuyoruz
        [Display(Name = "Çalışma Saatleri")]
        public string WorkingHours { get; set; } // Örn: "09:00-17:00"

        // İlişki: Antrenörün verdiği hizmet (Foreign Key)
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        // İlişki: Antrenörün randevuları
        public ICollection<Appointment> Appointments { get; set; }
    }
}