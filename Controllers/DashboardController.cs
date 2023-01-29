using Automation_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using RestSharp;

namespace Automation_Website.Controllers
{
    public class DashboardController : Controller
    {
        // GET: DashboardController
        public ActionResult Dashboard()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel()
            {
                Browsers = new List<Checkbox>
                {
                    new Checkbox { IsSelected = true, DisplayName = "Chrome", Value = "chrome" },
                    new Checkbox { IsSelected = true, DisplayName = "Micosoft Edge", Value = "edge" },
                    new Checkbox { IsSelected = true, DisplayName = "Firefox", Value = "firefox" },
                },
                OperatingSystems = new List<Checkbox>
                {
                    new Checkbox { IsSelected = true, DisplayName = "Ubuntu", Value = "ubuntu-20.04" },
                    new Checkbox { IsSelected = true, DisplayName = "Windows", Value = "windows-latest" },
                    new Checkbox { IsSelected = true, DisplayName = "Mac OS", Value = "macOS-latest" },
                },
                Emails = new List<Checkbox>
                {
                    new Checkbox { IsSelected = true, Value = "leoyuntao@gmail.com"},
                    new Checkbox { IsSelected = true, Value = "Windows@gmail.com" },
                },
                TestCases = new List<Checkbox>
                {
                    new Checkbox { IsSelected = true, Value = "test_apply_account" },
                    new Checkbox { IsSelected = true, Value = "test_apply_loan" },
                    new Checkbox { IsSelected = true, Value = "test_forgot_password" },
                    new Checkbox { IsSelected = true, Value = "test_loan_status" },
                    new Checkbox { IsSelected = true, Value = "test_login" },
                    new Checkbox { IsSelected = true, Value = "test_register" },
                    new Checkbox { IsSelected = true, Value = "test_transfer_funds" },
                },
            };

            return View(dashboardViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RunTest(DashboardViewModel dashboardViewModel)
        {
            var options = new RestClientOptions("https://api.github.com/repos/LeoYunTao/PFD-Team-A-Automation/actions/workflows/main.yml/dispatches")
            {
                MaxTimeout = -1
            };

            var client = new RestClient(options);

            var request = new RestRequest();
            request.AddHeader("Accept", "application/vnd.github+json");
            request.AddHeader("Authorization", "Bearer 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");
            request.AddHeader("X-GitHub-Api-Version", "2022-11-28");
            request.AddHeader("Content-Type", "application/json");

            var body = string.Format(@"{{""ref"": ""main"", {0} }}", dashboardViewModel.ToJson());
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            System.Diagnostics.Debug.WriteLine(body);

            RestResponse response = client.Post(request);
            System.Diagnostics.Debug.WriteLine(response.Content);

            return RedirectToAction("Dashboard");
        }
    }
}
