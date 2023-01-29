using System.ComponentModel.DataAnnotations;

namespace Automation_Website.Models
{
    public class DashboardViewModel
    {
        [Required]
        [Display(Name = "Operating System (OS)")]
        public List<Checkbox> OperatingSystems { get; set; }

        [Required]
        [Display(Name = "Test Cases")]
        public List<Checkbox> TestCases { get; set; }

        [Required]
        [Display(Name = "Browsers")]
        public List<Checkbox> Browsers { get; set; }

        [Required]
        [Display(Name = "Email")]
        public List<Checkbox> Emails { get; set; }

        public string ToString(List<Checkbox> checkboxList, char delimeter = ',', bool stringEnclose = false)
        {
            string list = "";
            for (int i = 0; i < checkboxList.Count; i++)
            {
                if (i > 0 && checkboxList[i].IsSelected && checkboxList[i-1].IsSelected)
                {
                    list += delimeter;
                }

                if (stringEnclose && checkboxList[i].IsSelected)
                {
                    list += "'" + checkboxList[i].ToJson() + "'";
                }
                else
                {
                    list += checkboxList[i].ToJson();
                }
            }
            
            return list;
        }

        public string ToJson()
        {
            string operatingSystems = ToString(OperatingSystems, stringEnclose: true);
            string testCases = ToString(TestCases);
            string browsers = ToString(Browsers);
            string emails = ToString(Emails, delimeter: ' ');

            return string.Format(@"""inputs"": {{""operatingSystems"": ""[{0}]"", ""testcases"": ""{1}"", ""browsers"": ""{2}"", ""emails"": ""{3}""}}", operatingSystems, testCases, browsers, emails);
        }
    }

    public class Checkbox
    {
        public string? DisplayName { get; set; }

        public string Value { get; set; } = string.Empty;

        public bool IsSelected { get; set; }

        public string ToJson()
        {
            return IsSelected ? Value : "";
        }
    }
}
