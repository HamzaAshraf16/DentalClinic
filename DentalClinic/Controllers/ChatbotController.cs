using DentalClinic.Models;
using FuzzySharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : Controller
    {
        private readonly Dictionary<string, string> _responses;

        public ChatbotController()
        {
            _responses = new Dictionary<string, string>
        {
            { "ساعات العمل", "عيادتنا تعمل من الاثنين الى السبت من الساعة الثانية عشر ظهرا الى الساعة الواحدة بعد منتصف الليل، يوم الأحد أجازة" },
            { "المواعيد", "عيادتنا تعمل من الاثنين الى السبت من الساعة الثانية عشر ظهرا الى الساعة الواحدة بعد منتصف الليل، يوم الأحد أجازة" },
            { "الخدمات", "تقدم العيادة العديد من الخدمات منها تنظيف الأسنان، حشو الأعصاب، حشو تجاويف الأسنان، جراحة زرع الأسنان، والعديد من الخدمات المختصة بتجميل الاسنان" },
            { "حجز", "لحجز موعد يمكنك الذهاب لصفحة الحجز واختيار الموعد المناسب" },
            { "حجز معاد", "لحجز موعد يمكنك الذهاب لصفحة الحجز واختيار الموعد المناسب" },
            { "معاد", "لحجز موعد يمكنك الذهاب لصفحة الحجز واختيار الموعد المناسب" },
            { "الفروع", "العيادة لها فرعان في مدينة القوصية وأبنوب، لمعرفة العناوين بالتفصيل اذهب لصفحة اتصل بنا" },
            { "دكتور مينا", "مواعيد دكتور مينا في فرع القوصية من الاثنين الى الاربعاء، وفي فرع أبنوب من الخميس الى السبت" },
            { "مينا", "مواعيد دكتور مينا في فرع القوصية من الاثنين الى الاربعاء، وفي فرع أبنوب من الخميس الى السبت" },
            { "دكتور مقار", "مواعيد دكتور مقار في فرع القوصية من الخميس الى السبت، وفي فرع أبنوب من الاثنين الى الاربعاء" },
            { "مقار", "مواعيد دكتور مقار في فرع القوصية من الخميس الى السبت، وفي فرع أبنوب من الاثنين الى الاربعاء" },
        };
        }

        [HttpPost]
        public IActionResult GetResponse([FromBody] ChatRequest request)
        {
            string lowercaseInput = request.Message.ToLower();
            string bestMatch = null;
            int highestScore = 0;

            foreach (var keyword in _responses.Keys)
            {
                int similarityScore = Fuzz.Ratio(lowercaseInput, keyword);

                if (similarityScore > highestScore)
                {
                    highestScore = similarityScore;
                    bestMatch = keyword;
                }
            }

            if (highestScore > 70 && bestMatch != null)
            {
                return Ok(new ChatResponse { Message = _responses[bestMatch] });
            }

            return Ok(new ChatResponse { Message = "عذراً، لا أملك معلومات حول ذلك. يرجى الاتصال بمكتبنا لطرح أسئلة محددة." });
        }
    }
}
