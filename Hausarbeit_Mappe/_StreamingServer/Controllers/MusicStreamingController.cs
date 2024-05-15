using Microsoft.AspNetCore.Mvc;

namespace _StreamingServer.Controllers
{
    public class MusicStreamingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
