using Automation_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Automation_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login (IFormCollection formData)
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

        public ActionResult LogOut() // Complete logout
        {
            //Compute login duration
            DateTime startTime = Convert.ToDateTime(HttpContext.Session.GetString("LoginTime"));
            DateTime endTime = DateTime.Now;
            TimeSpan loginDuration = endTime - startTime;
            string strLoginDuration = "";

            if (loginDuration.Days > 0)
                strLoginDuration += loginDuration.Days.ToString() + " days(s) ";

            if (loginDuration.Days > 0)
                strLoginDuration += loginDuration.Hours.ToString() + " hours(s) ";

            if (loginDuration.Days > 0)
                strLoginDuration += loginDuration.Minutes.ToString() + " minutes(s) ";

            if (loginDuration.Days > 0)
                strLoginDuration += loginDuration.Seconds.ToString() + " seconds ";
            TempData["LoginDuration"] = "You have logged in for " + strLoginDuration;

            // Clear all key-values pairs stored in session state
            HttpContext.Session.Clear();
            // Call the Index action of Home controller
            return RedirectToAction("Index");
        }
    }
}