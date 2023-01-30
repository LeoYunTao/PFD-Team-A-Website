using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Automation_Website.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dashboard(IFormCollection formData)
        {
            // Read inputs from textboxes
            // Email address converted to lowercase
            string username = formData["Username"].ToString().ToLower();
            string password = formData["Password"].ToString();
            if (username == "hi" && password == "hi")
            {
                // Redirect user to the "Dashboard" view through an action
                return RedirectToAction("Dashboard");
            }
            else
            {
                // Redirect user back to the index view through an action
                return RedirectToAction("Index");
            }
        }
        public ActionResult Dashboard()
        {
            return View();
        }
    }
    
}
