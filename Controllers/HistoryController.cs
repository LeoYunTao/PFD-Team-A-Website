using Automation_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Automation_Website.Controllers
{
    public class HistoryController : Controller
    {
        ApiCall apiCall = new ApiCall();

        public IActionResult Index(int id, string conclusion, string created)
        {
			if (HttpContext.Session.GetString("Role") == null) return RedirectToAction("Index", "Home");

			HistoryViewModel historyViewModel = new HistoryViewModel()
            {
                Conclusion = conclusion,
                Created = created,
            };

            string? conclusionStr = conclusion != null ? conclusion.ToLower() : conclusion;
            if (conclusionStr == "running")
            {
				conclusionStr = "in_progress";
            }
            else if (conclusionStr == "all")
            {
                conclusionStr = null;
            }

            WorkflowRuns workflowRuns = apiCall.GetWorkflowRuns(perPage: 10, pageNumber: id, status: conclusionStr, created: created);

            historyViewModel.SWorkflowRuns = workflowRuns;

            return View(historyViewModel);
        }
    }
}
