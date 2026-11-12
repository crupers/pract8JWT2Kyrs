using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prakt8JWT.BD;

namespace Prakt8JWT.Controllers
{
    [ApiController]
    [Route("api/v1/tasks")]
    [Authorize]
    public class TasksController : Controller
    {
        private readonly Prakt8Context _context;

        public TasksController(Prakt8Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var login = User.Identity?.Name;

            var tasks = await _context.Tasks
                .Where(t => t.OwnerLogin == login)
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            if (task.OwnerLogin != User.Identity?.Name &&
                !User.IsInRole("Admin"))
                return Forbid();

            return Ok(task);
        }

        public record CreateTaskRequest(string Title, string Description);

        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskRequest request)
        {
            var task = new Prakt8JWT.DbModels.Task
            {
                Title = request.Title,
                Description = request.Description,
                OwnerLogin = User.Identity!.Name!,
                IsCompleted = false
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask),
                new { id = task.Id }, task);
        }

        public record UpdateTaskRequest(
            string Title,
            string Description,
            bool IsCompleted);

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(
            int id, UpdateTaskRequest request)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            if (task.OwnerLogin != User.Identity?.Name &&
                !User.IsInRole("Admin"))
                return Forbid();

            task.Title = request.Title;
            task.Description = request.Description;
            task.IsCompleted = request.IsCompleted;

            await _context.SaveChangesAsync();

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                return NotFound();

            if (task.OwnerLogin != User.Identity?.Name &&
                !User.IsInRole("Admin"))
                return Forbid();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _context.Tasks.ToListAsync();

            return Ok(tasks);
        }

    }
}
