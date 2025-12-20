using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnessCenterProject.Areas.Admin.Controllers
{
    [Area("Admin")] // Admin alanında olduğunu belirtiyoruz
    [Authorize(Roles = "Admin")] // Sadece Admin yetkisi olanlar erişebilir
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME (READ)
        public async Task<IActionResult> Index()
        {
            // Antrenörleri getirirken bağlı oldukları Service (Hizmet) bilgisini de getiriyoruz (Include)
            var applicationDbContext = _context.Trainers.Include(t => t.Service);
            return View(await applicationDbContext.ToListAsync());
        }

        // 2. EKLEME SAYFASI (GET)
        public IActionResult Create()
        {
            // Formdaki Dropdown için hizmetleri yüklüyoruz
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            return View();
        }

        // 3. EKLEME İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialty,WorkingHours,ServiceId")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Hata olursa dropdown boş gelmesin diye tekrar dolduruyoruz
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", trainer.ServiceId);
            return View(trainer);
        }

        // 4. DÜZENLEME SAYFASI (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            // Mevcut hizmeti seçili getirmek için son parametreye trainer.ServiceId veriyoruz
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", trainer.ServiceId);
            return View(trainer);
        }

        // 5. DÜZENLEME İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Specialty,WorkingHours,ServiceId")] Trainer trainer)
        {
            if (id != trainer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", trainer.ServiceId);
            return View(trainer);
        }

        // 6. SİLME SAYFASI (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // 7. SİLME ONAYI (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}