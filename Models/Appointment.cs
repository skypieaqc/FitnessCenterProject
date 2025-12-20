using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Randevu Tarihi")]
        public DateTime AppointmentDate { get; set; }

        public bool IsConfirmed { get; set; } = false; // Onay durumu [cite: 21]

        // Hangi Üye?
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Hangi Antrenör?
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        // O anki hizmet neydi? (Fiyat değişebilir, kayıtta dursun)
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}