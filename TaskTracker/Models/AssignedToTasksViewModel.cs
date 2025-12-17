using System.Collections.Generic;
using System.Linq;

namespace TaskTracker.Models
{
    public class AssignedToTasksViewModel
    {
        public IEnumerable<UserTask> Tasks { get; set; } = Enumerable.Empty<UserTask>();
        public IEnumerable<string> Assignees { get; set; } = Enumerable.Empty<string>();
        public string? SelectedAssignee { get; set; }
    }
}

