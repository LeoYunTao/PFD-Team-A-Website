using Newtonsoft.Json;
using RestSharp;

namespace Automation_Website.Models
{
    public class ApiCall
    {
        private const string GITHUB_API_URL = "https://api.github.com/repos/LeoYunTao/PFD-Team-A-Automation/actions";
        
        public readonly string API_KEY = System.Configuration.ConfigurationManager.AppSettings["GITHUB_API_KEY"];

        RestRequest request;

        public ApiCall() 
        {
            request = new RestRequest();
            request.AddHeader("Accept", "application/vnd.github+json");
            request.AddHeader("Authorization", $"Bearer {API_KEY}");
            request.AddHeader("X-GitHub-Api-Version", "2022-11-28");
            request.AddHeader("Content-Type", "application/json");
        }

        public RestResponse RunTest(DashboardViewModel dashboardViewModel)
        {
            RestClient client = new RestClient($"{GITHUB_API_URL}/workflows/main.yml/dispatches");

            var body = string.Format(@"{{""ref"": ""main"", {0} }}", dashboardViewModel.ToJson());
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            RestResponse response = client.Post(request);

            return response;
        }

        public RestResponse CancelTest(string runId)
        {
            string cancelURL = $"{GITHUB_API_URL}/runs/{runId}/cancel";

            RestClient client = new RestClient(cancelURL);

            RestResponse response = client.Post(request);

            return response; 
        }

        public WorkflowRun GetWorkflowRun(string runId)
        {
            string workflowRunURL = $"{GITHUB_API_URL}/runs/{runId}";
            
            WorkflowRun workflowRun = GetRequest<WorkflowRun>(workflowRunURL);

            return workflowRun;
        }

        public WorkflowRuns GetWorkflowRuns(int perPage = 30, int pageNumber = 1, string? status = null, string? created = null)
        {
            string workflowRunsURL = $"{GITHUB_API_URL}/workflows/main.yml/runs?per_page={perPage}&page={pageNumber}"; ;
            
            if (status != null)
            {
                workflowRunsURL += $"&status={status}";
			}

            if (created != null)
            {
                workflowRunsURL += $"&created={created}";
            }

			WorkflowRuns workflowRuns = GetRequest<WorkflowRuns>(workflowRunsURL);

            return workflowRuns;
        }

        public T GetRequest<T>(string url) where T : class
        {
            RestResponse response = GetRequest(url);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public RestResponse GetRequest(string url)
        {
            RestClient client = new RestClient(url);

            RestResponse response = client.Get(request);

            return response;
        }
    }
}
