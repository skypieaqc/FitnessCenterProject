using System.Security.Claims;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterProject.Controllers
{
    [Authorize] // Sadece giriş yapmış kullanıcılar randevu alabilir
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. RANDEVULARIM SAYFASI (Geçmiş randevuları listeler)
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Where(a => a.UserId == userId) // Sadece benim randevularım
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        // 2. RANDEVU ALMA SAYFASI (GET)
        public IActionResult Create(int? serviceId)
        {
            // Eğer hizmet seçilmeden gelindiyse ana sayfaya at
            if (serviceId == null) return RedirectToAction("Index", "Home");

            // Seçilen hizmeti bul
            var service = _context.Services.Find(serviceId);
            if (service == null) return NotFound();

            // Sadece bu hizmeti veren antrenörleri getir
            var trainers = _context.Trainers.Where(t => t.ServiceId == serviceId).ToList();

            ViewBag.ServiceName = service.Name;
            ViewBag.ServiceId = serviceId;

            // Dropdown için antrenör listesi
            ViewData["TrainerId"] = new SelectList(trainers, "Id", "FullName");

            return View();
        }

        // 3. RANDEVU KAYDETME (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // Kullanıcı ID'sini al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            appointment.UserId = userId;

            // Antrenör seçildi mi?
            if (appointment.TrainerId == 0)
            {
                ModelState.AddModelError("", "Lütfen bir antrenör seçiniz.");
            }

            // --- KRİTİK KONTROL: Antrenör o saatte dolu mu? ---
            // [cite: 20] Randevu saati uygun değilse uyar.
            bool isBooked = await _context.Appointments.AnyAsync(a =>
                a.TrainerId == appointment.TrainerId &&
                a.AppointmentDate == appointment.AppointmentDate);

            if (isBooked)
            {
                ModelState.AddModelError("", "Seçtiğiniz antrenörün bu tarih ve saatte başka bir randevusu var. Lütfen farklı bir zaman seçiniz.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Randevularım sayfasına git
            }

            // Hata varsa sayfayı tekrar doldur
            var service = _context.Services.Find(appointment.ServiceId);
            ViewBag.ServiceName = service?.Name;
            ViewBag.ServiceId = appointment.ServiceId;
            var trainers = _context.Trainers.Where(t => t.ServiceId == appointment.ServiceId).ToList();
            ViewData["TrainerId"] = new SelectList(trainers, "Id", "FullName", appointment.TrainerId);

            return View(appointment);
        }
    }
}