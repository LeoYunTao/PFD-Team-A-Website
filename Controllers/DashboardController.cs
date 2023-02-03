using Automation_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Automation_Website.Controllers
{
    public class DashboardController : Controller
    {
        ApiCall apiCall = new ApiCall();

        public ActionResult Dashboard()
        {
			if (HttpContext.Session.GetString("Role") == null) return RedirectToAction("Index", "Home");

			HttpContext.Session.SetString("api", apiCall.API_KEY);

            
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

            if (HttpContext.Session.GetString("emailList") == null)
            {
                string emailListString = JsonConvert.SerializeObject(dashboardViewModel.Emails);

                HttpContext.Session.SetString("emailList", emailListString);
            }

            return View(dashboardViewModel);
        }


        [HttpPost]
        public ActionResult Dashboard(IFormCollection formData)
        {
            // Read inputs from textboxes
            // Username converted to lowercase
            string username = formData["Username"].ToString();
            string password = formData["Password"].ToString();
            if (username == "Rakesh" && password == "1234")
            {

                HttpContext.Session.SetString("Username", username);
                // Store user Role “Staff” as a string in session with the key “Role”
                HttpContext.Session.SetString("Role", "Automation Manager");
                HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());
                // Redirect user to the "Dashboard" view through an action
                return RedirectToAction("Dashboard");
            }
            else
            {
                TempData["Message"] = "Invalid Login Credentials!";
                // Redirect user back to the index view through an action
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult AddEmail(IFormCollection formCollection)
        {
			if (HttpContext.Session.GetString("emailList") == null)
			{
				string emailListString = JsonConvert.SerializeObject(new List<Checkbox>());

				HttpContext.Session.SetString("emailList", emailListString);
			}

			string emailListStr = HttpContext.Session.GetString("emailList");

			List<Checkbox> emailList = JsonConvert.DeserializeObject<List<Checkbox>>(emailListStr);

			emailList.Add(new Checkbox { IsSelected = true, Value = formCollection["email"] });

            HttpContext.Session.SetString("emailList", JsonConvert.SerializeObject(emailList));

			return StatusCode(200);
		}

        [HttpPost]
        public ActionResult RemoveEmail(IFormCollection formCollection)
        {
			string emailListStr = HttpContext.Session.GetString("emailList");

			List<Checkbox> emailList = JsonConvert.DeserializeObject<List<Checkbox>>(emailListStr);

			string emailToDelete = formCollection["email"].ToString();

            //foreach (var i in formCollection.Keys)
            //{
            //    System.Diagnostics.Debug.WriteLine(i);
            //}

            //System.Diagnostics.Debug.WriteLine(emailToDelete);

            foreach (Checkbox checkbox in emailList)
            {
                if (emailToDelete == checkbox.Value)
                {
					emailList.Remove(checkbox);
                    break;
                }
            }

            HttpContext.Session.SetString("emailList", JsonConvert.SerializeObject(emailList));

			return StatusCode(200);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TestStatus(DashboardViewModel dashboardViewModel)
        {
			string emailListStr = HttpContext.Session.GetString("emailList");

            if (emailListStr != null)
            {
				List<Checkbox> emailList = JsonConvert.DeserializeObject<List<Checkbox>>(emailListStr);

				dashboardViewModel.Emails = emailList;

			}

			System.Diagnostics.Debug.WriteLine(emailListStr);

			apiCall.RunTest(dashboardViewModel);

            Thread.Sleep(5000);

            WorkflowRuns workflowRuns = apiCall.GetWorkflowRuns(perPage: 1);

            string id = workflowRuns.workflow_runs[0].id.ToString();

            return RedirectToAction("TestStatus", new { id = id });
        }

        public ActionResult TestStatus(string id)
        {
			if (HttpContext.Session.GetString("Role") == null) return RedirectToAction("Index", "Home");

			HttpContext.Session.SetString("api", apiCall.API_KEY);

			WorkflowRun workflowRun = apiCall.GetWorkflowRun(id);

            string conclustion = workflowRun.conclusion == null ? "running" : workflowRun.conclusion;

            DateTime timeStarted = workflowRun.created_at;
            DateTime? timeFinished = workflowRun.updated_at;

            string status = workflowRun.status;
            if (status != "completed")
            {
                timeFinished = null;
            }

            string workflowRunJobsURL = workflowRun.jobs_url;
            WorkflowRunJobs workflowRunJobs = apiCall.GetRequest<WorkflowRunJobs>(workflowRunJobsURL);

            Dictionary<string, List<Job>> workflowJobsDictionary = GenerateWorkflowJobsDictionary(workflowRunJobs);

            List<TestStatus> testStatusList = GenerateTestStatusList(workflowJobsDictionary);

            TestStatusViewModel testStatusViewModel = new TestStatusViewModel()
            {
                RunId = id,
                TimeStarted = timeStarted,
                TimeFinished = timeFinished,
                TestStatusList = testStatusList,
                Conclusion = conclustion,
            };

            if (conclustion == "success")
            {
                string artifactsURL = workflowRun.artifacts_url;

                List<Artifact> artifactList = apiCall.GetRequest<Artifacts>(artifactsURL).artifacts;
                testStatusViewModel.ArtifactList = artifactList;
            }

            return View(testStatusViewModel);
        }

        [HttpPost]
        public ActionResult CancelTest(string id)
        {
            apiCall.CancelTest(id);

            return RedirectToAction("TestStatus", new { id = id });
        }

        private List<TestStatus> GenerateTestStatusList(Dictionary<string, List<Job>> workflowJobsDictionary)
        {
            List<TestStatus> testStatusList = new List<TestStatus>();

            foreach (KeyValuePair<string, List<Job>> keyValuePair in workflowJobsDictionary)
            {
                TestStatus testStatus = new TestStatus()
                {
                    Name = keyValuePair.Key
                };

                string statusName = "queued";

                IEnumerable<IGrouping<string, Job>> statusCount = keyValuePair.Value.GroupBy(i => i.status);
                foreach (IGrouping<string, Job> status in statusCount)
                {
                    if (status.Key == "in_progress" && status.Count() > 0)
                    {
                        statusName = status.Key;
                        break;
                    }
                    else if (status.Key == "completed" && keyValuePair.Value.Count() == status.Count())
                    {
                        statusName = status.Key;
                        break;
                    }

                }


                testStatus.Status = statusName;

                testStatusList.Add(testStatus);
            }

            return testStatusList;
        }

        private Dictionary<string, List<Job>> GenerateWorkflowJobsDictionary(WorkflowRunJobs workflowRunJobs)
        {
            Dictionary<string, List<Job>> workflowJobsDictionary = new Dictionary<string, List<Job>>()
            {
                { "build", new List<Job>() },
                { "generate", new List<Job>() },
                { "send", new List<Job>() },
            };

            foreach (Job job in workflowRunJobs.jobs)
            {
                string keyName = job.name.ToLower().Split()[0];
                workflowJobsDictionary[keyName].Add(job);
            }

            return workflowJobsDictionary;
        }

    }
}
