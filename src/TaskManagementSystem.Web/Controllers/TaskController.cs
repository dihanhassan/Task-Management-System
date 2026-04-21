using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Core.Enums;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.ViewModels;

namespace TaskManagementSystem.Web.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly ITaskService _taskService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TaskController> _logger;

    private const int DefaultPageSize = 10;

    public TaskController(ITaskService taskService, IConfiguration configuration, ILogger<TaskController> logger)
    {
        _taskService = taskService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string searchTerm = "",
        TaskFilter filter = TaskFilter.All,
        TaskSort sort = TaskSort.DueDateAsc,
        int page = 1)
    {
        var userId = GetCurrentUserId();
        var pageSize = GetPageSize();
        var viewModel = await _taskService.GetTaskListAsync(userId, searchTerm, filter, sort, page, pageSize);
        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TaskCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = GetCurrentUserId();
        await _taskService.CreateAsync(model, userId);
        TempData["SuccessMessage"] = "Task created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = GetCurrentUserId();
        var viewModel = await _taskService.GetForEditAsync(id, userId);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TaskEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = GetCurrentUserId();
        await _taskService.UpdateAsync(model, userId);
        TempData["SuccessMessage"] = "Task updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        await _taskService.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Task deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var userId = GetCurrentUserId();
        var isCompleted = await _taskService.ToggleStatusAsync(id, userId);
        return Json(new { success = true, isCompleted });
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term = "")
    {
        var userId = GetCurrentUserId();
        var pageSize = GetPageSize();
        var viewModel = await _taskService.GetTaskListAsync(userId, term, TaskFilter.All, TaskSort.DueDateAsc, 1, pageSize);
        return PartialView("_TaskTableRows", viewModel.Tasks);
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv()
    {
        var userId = GetCurrentUserId();
        var csvBytes = await _taskService.ExportToCsvAsync(userId);
        var fileName = $"tasks-{DateTime.Today:yyyy-MM-dd}.csv";
        _logger.LogInformation("CSV export requested. UserId={UserId}", userId);
        return File(csvBytes, "text/csv", fileName);
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    private int GetPageSize()
    {
        return _configuration.GetValue<int>("Pagination:PageSize", DefaultPageSize);
    }
}
