using Automation_Website.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RestSharp;

namespace Automation_Website.Controllers
{
    public class DashboardController : Controller
    {
        // GET: DashboardController

        private const string GITHUB_API_URL = "https://api.github.com/repos/LeoYunTao/PFD-Team-A-Automation/actions";


        public ActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("dashboardViewModel") == null)
            {
                DashboardViewModel newDashboardViewModel = new DashboardViewModel()
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

                HttpContext.Session.SetString("dashboardViewModel", JsonConvert.SerializeObject(newDashboardViewModel));
            }

            string str = HttpContext.Session.GetString("dashboardViewModel");

            DashboardViewModel dashboardViewModel = JsonConvert.DeserializeObject<DashboardViewModel>(str);

            return View(dashboardViewModel);
        }

        [HttpPost]
        public void AddEmail(IFormCollection formCollection)
        {
            string str = HttpContext.Session.GetString("dashboardViewModel");

            DashboardViewModel dashboardViewModel = JsonConvert.DeserializeObject<DashboardViewModel>(str);

            dashboardViewModel.Emails.Add(new Checkbox { IsSelected = true, Value = formCollection["email"] });

            HttpContext.Session.SetString("dashboardViewModel", JsonConvert.SerializeObject(dashboardViewModel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TestStatus(DashboardViewModel dashboardViewModel)
        {
            RestClient client = new RestClient($"{GITHUB_API_URL}/workflows/main.yml/dispatches");

            RestRequest request = SetUpRestRequest();

            var body = string.Format(@"{{""ref"": ""main"", {0} }}", dashboardViewModel.ToJson());
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            RestResponse response = client.Post(request);

            Thread.Sleep(5000);

            //System.Diagnostics.Debug.WriteLine(response.ResponseStatus);
            //System.Diagnostics.Debug.WriteLine(response.Content);

            client = new RestClient($"{GITHUB_API_URL}/workflows/main.yml/runs?per_page=1");
            response = client.Get(request);

            WorkflowRuns workflowRuns = JsonConvert.DeserializeObject<WorkflowRuns>(response.Content);

            string id = workflowRuns.workflow_runs[0].id.ToString();

            return RedirectToAction("TestStatus", new { id = id });
        }

        public ActionResult TestStatus(string id)
        {
            string workflowRunURL = $"{GITHUB_API_URL}/runs/{id}";
            WorkflowRun workflowRun = GetRequest<WorkflowRun>(workflowRunURL);
            
            string conclustion = workflowRun.conclusion == null ? "running" : workflowRun.conclusion;

            DateTime timeStarted = workflowRun.created_at;
            DateTime? timeFinished = workflowRun.updated_at;

            string status = workflowRun.status;
            if (status != "completed")
            {
                timeFinished = null;
            }

            string workflowRunJobsURL = workflowRun.jobs_url;
            WorkflowRunJobs workflowRunJobs = GetRequest<WorkflowRunJobs>(workflowRunJobsURL);

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

                List<Artifact> artifactList = GetRequest<Artifacts>(artifactsURL).artifacts;
                testStatusViewModel.ArtifactList = artifactList;
            }

            return View(testStatusViewModel);
        }

        [HttpPost]
        public ActionResult CancelTest(string id)
        {
            string cancelURL = $"{GITHUB_API_URL}/runs/{id}/cancel";

            RestClient client = new RestClient(cancelURL);

            RestRequest request = SetUpRestRequest();

            RestResponse response = client.Post(request);

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

        private T GetRequest<T>(string url) where T : class
        {
            RestResponse response = GetRequest(url);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private RestResponse GetRequest(string url)
        {
            RestClient client = new RestClient(url);

            RestRequest request = SetUpRestRequest();
            RestResponse response = client.Get(request);

            return response;
        }

        public RestRequest SetUpRestRequest()
        {
            RestRequest request = new RestRequest();
            request.AddHeader("Accept", "application/vnd.github+json");
            request.AddHeader("Authorization", "Bearer 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");
            request.AddHeader("X-GitHub-Api-Version", "2022-11-28");
            request.AddHeader("Content-Type", "application/json");

            return request;
        }
    }
}
