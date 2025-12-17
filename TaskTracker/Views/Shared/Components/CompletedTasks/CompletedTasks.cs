using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;

namespace TaskTracker.Views.Shared.Components.CompletedTasks
{
    public class CompletedTasks : ViewComponent
    {
        private readonly ToDoListDbContext _context;

        public CompletedTasks(ToDoListDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var startDate = DateTime.Now.AddDays(-7); // last week
            var endDate = DateTime.Now;

            var completedTasks = _context.Tasks
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate && t.IsCompleted == true)
                .ToList();

            return View(completedTasks);
        }
    }
}
