namespace To_Do.Services
{
    public class TaskPrioritizer
    {
        private readonly ILogger _logger;

        public TaskPrioritizer(ILogger<TaskPrioritizer> logger)
        {
            _logger = logger;
        }

        public List<To_Do.Models.UserTask> PrioritizeTasks(List<To_Do.Models.UserTask> tasks)
        {
            // Call the QuickSort method
            QuickSort(tasks, 0, tasks.Count - 1);
            return tasks;
        }

        private void QuickSort(List<To_Do.Models.UserTask> tasks, int low, int high)
        {
            if (low < high)
            {
                // Partition the list
                int pivotIndex = Partition(tasks, low, high);

                // Recursively sort elements before and after partition
                QuickSort(tasks, low, pivotIndex - 1);
                QuickSort(tasks, pivotIndex + 1, high);
            }
        }

        private int Partition(List<To_Do.Models.UserTask> tasks, int low, int high)
        {
            // Choose the rightmost element as the pivot
            var pivot = tasks[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                // Compare deadlines first, then priority
                if (tasks[j].Deadline < pivot.Deadline ||
                    (tasks[j].Deadline == pivot.Deadline && tasks[j].Priority < pivot.Priority))
                {
                    i++;
                    Swap(tasks, i, j);
                }
            }
            Swap(tasks, i + 1, high);
            return i + 1;
        }

        private void Swap(List<To_Do.Models.UserTask> tasks, int i, int j)
        {
            var temp = tasks[i];
            tasks[i] = tasks[j];
            tasks[j] = temp;
        }
    }
}