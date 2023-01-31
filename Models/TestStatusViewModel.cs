using System.Reflection;

namespace Automation_Website.Models
{
    public class TestStatusViewModel
    {
        public string RunId { get; set; } = string.Empty;

        public DateTime TimeStarted { get; set; }

        public DateTime? TimeFinished { get; set; }

        public string Conclusion { get; set; } = string.Empty;

        public List<TestStatus> TestStatusList { get; set; }

        public List<Artifact>? ArtifactList { get; set; }

        public string FormatTime(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"d\d\ hh\h\ mm\m\i\n\ s\s\e\c")
                .TrimStart(' ', 'd', 'h', 'm', 's', '0');
        }
    }

    public class TestStatus
    {
        public string Name { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
