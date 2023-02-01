using Microsoft.AspNetCore.Mvc;

namespace Automation_Website.Controllers
{
    public class HistoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
