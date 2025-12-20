using FitnessCenterProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterProject.Controllers.Api
{
    [Route("api/[controller]")] // Tarayıcıdan erişim adresi: /api/trainersapi
    [ApiController]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TÜM ANTRENÖRLERİ GETİR
        // İstek: GET /api/trainersapi
        [HttpGet]
        public async Task<IActionResult> GetAllTrainers()
        {
            // Döngüye girmemesi için (Circular Reference) sadece ihtiyacımız olan alanları seçiyoruz.
            var trainers = await _context.Trainers
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    Uzmanlik = t.Specialty,
                    Hizmet = t.Service.Name
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // 2. BELİRLİ TARİHTE UYGUN OLANLARI GETİR (LINQ FİLTRELEME)
        // İstek: GET /api/trainersapi/available?date=2025-12-25T14:00
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTrainers(DateTime date)
        {

            // 1. O tarihte ve saatte dolu olan antrenörlerin ID'lerini bul
            var bookedTrainerIds = await _context.Appointments
                .Where(a => a.AppointmentDate == date)
                .Select(a => a.TrainerId)
                .ToListAsync();

            // 2. Dolu OLMAYAN antrenörleri filtrele
            var availableTrainers = await _context.Trainers
                .Where(t => !bookedTrainerIds.Contains(t.Id)) // LINQ: İçinde olmayanlar
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    Uzmanlik = t.Specialty,
                    Musaitlik = "Uygun"
                })
                .ToListAsync();

            return Ok(availableTrainers);
        }
    }
}