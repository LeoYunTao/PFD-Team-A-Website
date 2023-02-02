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

            List<Checkbox> selectedCheckbox = checkboxList.FindAll(checkbox => checkbox.IsSelected);

            string list = "";
            for (int i = 0; i < selectedCheckbox.Count; i++)
            {
                if (i > 0 && selectedCheckbox.Count > 1)
                {
                    list += delimeter;
                }

                if (stringEnclose)
                {
                    list += "'" + selectedCheckbox[i].ToJson() + "'";
                }
                else
                {
                    list += selectedCheckbox[i].ToJson();
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
