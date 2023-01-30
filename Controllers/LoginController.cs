using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Automation_Website.Models;

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
            // Username converted to lowercase
            string username = formData["Username"].ToString();
            string password = formData["Password"].ToString();
            if (username == "hi" && password == "hi")
            {

                HttpContext.Session.SetString("Username", username);
                // Store user role “Staff” as a string in session with the key “Role”
                HttpContext.Session.SetString("Role", "Automation Manager");
                HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());
                // Redirect user to the "Dashboard" view through an action
                return RedirectToAction("Dashboard");
            }
            else
            {
                TempData["Message"] = "Invalid Login Credentials!";
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
