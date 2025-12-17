using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskTracker.Models;
using TaskTracker.Services;
using TaskModel = TaskTracker.Models.UserTask;

namespace TaskTracker.Controllers
{
    public class TasksController : Controller
    {
        private readonly ToDoListDbContext _context;
        private readonly TaskPrioritizer _taskPrioritizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ToDoListDbContext context, TaskPrioritizer taskPrioritizer, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _taskPrioritizer = taskPrioritizer;
            _userManager = userManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(string sortOrder, string status)
        {
            ViewData["DeadlineSortParm"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";
            ViewData["PrioritySortParm"] = sortOrder == "Priority" ? "priority_desc" : "Priority";

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not found
            }

            // Fetch tasks specific to the logged-in user
            var tasks = _context.Tasks.Where(t => t.UserId == user.Id);

            // Filter by completion status
            if (status == "completed")
            {
                tasks = tasks.Where(t => t.IsCompleted);
            }
            else if (status == "incomplete")
            {
                tasks = tasks.Where(t => !t.IsCompleted);
            }
            // else show all

            // Sorting logic
            switch (sortOrder)
            {
                case "Deadline":
                    tasks = tasks.OrderBy(s => s.Deadline);
                    break;
                case "deadline_desc":
                    tasks = tasks.OrderByDescending(s => s.Deadline);
                    break;
                case "Priority":
                    tasks = tasks.OrderBy(s => s.Priority);
                    break;
                case "priority_desc":
                    tasks = tasks.OrderByDescending(s => s.Priority);
                    break;
                default:
                    tasks = tasks.OrderBy(s => s.Deadline);
                    break;
            }

            var tasksList = await tasks.ToListAsync();
            var prioritizedTasks = _taskPrioritizer.PrioritizeTasks(tasksList);

            return View(prioritizedTasks);
        }

        public async Task<IActionResult> AssignedToTasks(string? assignee)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Tasks
                .Where(t => t.UserId == user.Id && !string.IsNullOrWhiteSpace(t.AssignedTo));

            var assigneeOptions = await query
                .Select(t => t.AssignedTo!)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();

            var resultTasks = new List<UserTask>();

            if (!string.IsNullOrWhiteSpace(assignee))
            {
                resultTasks = await query
                    .Where(t => t.AssignedTo == assignee)
                    .OrderBy(t => t.Deadline)
                    .ThenByDescending(t => t.Priority)
                    .ToListAsync();
            }

            var viewModel = new AssignedToTasksViewModel
            {
                SelectedAssignee = assignee,
                Assignees = assigneeOptions,
                Tasks = resultTasks
            };

            return View(viewModel);
        }

        public IActionResult TaskCompletionRate()
        {
            var startDate = DateTime.Now.AddDays(-7); // last week
            var endDate = DateTime.Now;

            var totalTasks = _context.Tasks
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate)
                .Count();

            var completedTasks = _context.Tasks
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate && t.IsCompleted == true)
                .Count();

            var pendingTasks = _context.Tasks
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate && t.IsCompleted == false && t.Deadline >= endDate)
                .Count();

            var notCompletedTasks = _context.Tasks
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate && t.IsCompleted == false && t.Deadline < endDate)
                .Count();

            double completionRate = (double)(completedTasks) / (totalTasks == 0 ? 1 : totalTasks) * 100;
            double pendingRate = (double)(pendingTasks) / (totalTasks == 0 ? 1 : totalTasks) * 100;
            double notcompletedRate = (double)(notCompletedTasks) / (totalTasks == 0 ? 1 : totalTasks) * 100;


            return View(new TaskCompletionRateViewModel
            {
                CompletionRate = Math.Round(completionRate, 2),
                PendingRate = Math.Round(pendingRate, 2),
                notCompletedRate = Math.Round(notcompletedRate, 2)
            });
        }

        public IActionResult CompletedTasks()
        {
            var startDate = DateTime.Now.AddDays(-7); // last week
            var endDate = DateTime.Now;
            var completedTasks = _context.Tasks
                .Where(t => t.CreatedDate >= startDate && t.CreatedDate <= endDate && t.IsCompleted == true)
                .ToList();

/*            return ViewComponent("CompletedTasks");*/
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkTaskAsCompleted(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                task.IsCompleted = true;
                task.CompletedDate = DateTime.Now;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<string>> GetExistingAssigneesAsync()
        {
            return await _context.Tasks
                .Where(t => !string.IsNullOrWhiteSpace(t.AssignedTo))
                .Select(t => t.AssignedTo!)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();
        }

        private async Task PopulateExistingAssigneesAsync()
        {
            ViewBag.ExistingAssignees = await GetExistingAssigneesAsync();
        }

        // GET: Tasks/Create
        public async Task<IActionResult> Create()
        {
            await PopulateExistingAssigneesAsync();
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskModel task, string? selectedAssignee)
        {
            // Get the UserId from the claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "User Id cannot be null or empty.");
                await PopulateExistingAssigneesAsync();
                return View(task);
            }
            // Find the user in the database
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                // Handle the case where the user is not found
                ModelState.AddModelError("", "User  not found.");
                await PopulateExistingAssigneesAsync();
                return View(task);
            }
            Console.WriteLine("userid isss " + user);
            // Set the UserId and User properties
            task.UserId = userId; // Set the UserId
            task.User = user; // Set the User navigation property

            if (string.IsNullOrWhiteSpace(task.AssignedTo) && !string.IsNullOrWhiteSpace(selectedAssignee))
            {
                task.AssignedTo = selectedAssignee;
            }
            else if (!string.IsNullOrWhiteSpace(task.AssignedTo))
            {
                task.AssignedTo = task.AssignedTo.Trim();
            }

            if (ModelState.IsValid)
            {
                task.CreatedDate = DateTime.Now;
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync(); // Use async method for saving changes
                return RedirectToAction(nameof(Index));
            }

            // Log the errors for debugging
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            await PopulateExistingAssigneesAsync();
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskModel task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingTask = await _context.Tasks.FindAsync(id);
                if (existingTask == null)
                {
                    return NotFound();
                }

                existingTask.Title = task.Title;
                existingTask.Description = task.Description;
                existingTask.Deadline = task.Deadline;
                existingTask.Priority = task.Priority;
                existingTask.IsCompleted = task.IsCompleted;
                existingTask.AssignedTo = task.AssignedTo;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Delete/5
        public IActionResult Delete(int id)
        {
            var task = _context.Tasks
                .FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var task = _context.Tasks.Find(id);
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
