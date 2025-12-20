using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [Display(Name = "Hizmet Adı")]
        public string Name { get; set; } // Örn: Yoga, Pilates

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Display(Name = "Süre (Dakika)")]
        public int DurationMinutes { get; set; } // Seans süresi

        [Display(Name = "Ücret")]
        public decimal Price { get; set; }

        // İlişki: Bu hizmeti veren antrenörler
        public ICollection<Trainer> Trainers { get; set; }
    }
}