using Microsoft.AspNetCore.Mvc;

namespace DSWI_cibertec_demo.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
