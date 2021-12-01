using Microsoft.AspNetCore.Mvc;

namespace HE2.Controllers
{
    public class BFVController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}