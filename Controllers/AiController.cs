using FitnessCenterProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// Yeni eklenen kütüphaneler
using Google.GenAI;
using Google.GenAI.Types;
using System.Threading.Tasks;

namespace FitnessCenterProject.Controllers
{
    [Authorize]
    public class AiController : Controller
    {
        // GÜVENLİK UYARISI: Bu projeyi GitHub'a yüklemeden önce bu anahtarı buradan silmelisin!
        private readonly string _apiKey = "";

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePlan(int height, int weight, string goal, string gender)
        {
            // Kullanıcı Prompt'u
            string userPrompt = $"Ben {height} cm boyunda, {weight} kg ağırlığında bir {gender} bireyim. " +
                                $"Hedefim: {(goal == "muscle" ? "Kas Yapmak" : goal == "weightloss" ? "Kilo Vermek" : "Form Korumak")}. " +
                                $"Bana maddeler halinde kısa bir diyet listesi ve haftalık egzersiz programı önerir misin? " +
                                $"Cevabı HTML formatında (sadece <p> ve <ul> etiketleri kullanarak) ver.";

            string aiResponse = "";

            try
            {
                // 1. Ortam değişkeni (Environment) satırını tamamen SİLİYORUZ.
                // Çakışma hatası ve isim karmaşası böylece biter.

                // 2. Anahtarı doğrudan Client'ın içine veriyoruz.
                // Bu yöntem en garantisidir.
                var client = new Client(apiKey: _apiKey);

                // 3. İsteği gönder
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: userPrompt
                );

                // 4. Cevabı al
                if (response != null && response.Candidates.Count > 0)
                {
                    aiResponse = response.Candidates[0].Content.Parts[0].Text;

                    // Markdown temizliği
                    aiResponse = aiResponse.Replace("```html", "").Replace("```", "").Trim();
                }
                else
                {
                    aiResponse = "Yapay zeka boş bir cevap döndü.";
                }
            }
            catch (Exception ex)
            {
                aiResponse = $"HATA: Bir sorun oluştu. Detay: {ex.Message}";
            }

            ViewBag.AiResponse = aiResponse;

            // BMI Hesapla
            double heightM = height / 100.0;
            ViewBag.Bmi = heightM > 0 ? Math.Round(weight / (heightM * heightM), 1) : 0;
            ViewBag.Status = "Analiz Edildi";

            return View("Result");
        }
    }
}