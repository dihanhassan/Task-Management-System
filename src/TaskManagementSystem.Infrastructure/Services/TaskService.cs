using Microsoft.Extensions.Logging;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Enums;
using TaskManagementSystem.Core.Exceptions;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.ViewModels;
using TaskManagementSystem.Infrastructure.Helpers;
using X.PagedList.Extensions;

namespace TaskManagementSystem.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository repository, ILogger<TaskService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TaskListViewModel> GetTaskListAsync(
        string userId, string searchTerm, TaskFilter filter, TaskSort sort, int page, int pageSize)
    {
        try
        {
            var tasks = await _repository.GetByUserAsync(userId, searchTerm, filter, sort);
            var counts = await _repository.GetCountsByUserAsync(userId);

            var pagedRows = tasks.Select(TaskMapper.ToRowViewModel).ToPagedList(page, pageSize);

            return new TaskListViewModel
            {
                Tasks = pagedRows,
                SearchTerm = searchTerm,
                ActiveFilter = filter,
                ActiveSort = sort,
                Dashboard = TaskMapper.ToDashboardViewModel(counts)
            };
        }
        catch (TaskNotFoundException) { throw; }
        catch (UnauthorizedTaskAccessException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load task list. UserId={UserId}", userId);
            throw;
        }
    }

    public async Task<TaskEditViewModel> GetForEditAsync(int taskId, string userId)
    {
        try
        {
            var task = await GetVerifiedTaskAsync(taskId, userId);
            return TaskMapper.ToEditViewModel(task);
        }
        catch (TaskNotFoundException) { throw; }
        catch (UnauthorizedTaskAccessException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get task for edit. TaskId={TaskId}, UserId={UserId}", taskId, userId);
            throw;
        }
    }

    public async Task CreateAsync(TaskCreateViewModel model, string userId)
    {
        try
        {
            var task = TaskMapper.ToEntity(model, userId);
            await _repository.CreateAsync(task);
            _logger.LogInformation("Task created. UserId={UserId}, Title={Title}", userId, model.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create task. UserId={UserId}, Title={Title}", userId, model.Title);
            throw;
        }
    }

    public async Task UpdateAsync(TaskEditViewModel model, string userId)
    {
        try
        {
            var task = await GetVerifiedTaskAsync(model.Id, userId);
            TaskMapper.ApplyEdit(task, model);
            await _repository.UpdateAsync(task);
            _logger.LogInformation("Task updated. TaskId={TaskId}, UserId={UserId}", model.Id, userId);
        }
        catch (TaskNotFoundException) { throw; }
        catch (UnauthorizedTaskAccessException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update task. TaskId={TaskId}, UserId={UserId}", model.Id, userId);
            throw;
        }
    }

    public async Task DeleteAsync(int taskId, string userId)
    {
        try
        {
            var task = await GetVerifiedTaskAsync(taskId, userId);
            await _repository.DeleteAsync(task);
            _logger.LogInformation("Task deleted. TaskId={TaskId}, UserId={UserId}", taskId, userId);
        }
        catch (TaskNotFoundException) { throw; }
        catch (UnauthorizedTaskAccessException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete task. TaskId={TaskId}, UserId={UserId}", taskId, userId);
            throw;
        }
    }

    public async Task<bool> ToggleStatusAsync(int taskId, string userId)
    {
        try
        {
            await GetVerifiedTaskAsync(taskId, userId);
            var isCompleted = await _repository.ToggleStatusAsync(taskId);
            _logger.LogInformation("Task toggled. TaskId={TaskId}, IsCompleted={IsCompleted}", taskId, isCompleted);
            return isCompleted;
        }
        catch (TaskNotFoundException) { throw; }
        catch (UnauthorizedTaskAccessException) { throw; }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle task. TaskId={TaskId}, UserId={UserId}", taskId, userId);
            throw;
        }
    }

    public async Task<byte[]> ExportToCsvAsync(string userId)
    {
        try
        {
            var tasks = await _repository.GetByUserAsync(userId, string.Empty, TaskFilter.All, TaskSort.DueDateAsc);
            return CsvExportHelper.GenerateCsv(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export CSV. UserId={UserId}", userId);
            throw;
        }
    }

    private async Task<TaskItem> GetVerifiedTaskAsync(int taskId, string userId)
    {
        var task = await _repository.GetByIdAsync(taskId)
            ?? throw new TaskNotFoundException(taskId);

        if (task.UserId != userId)
            throw new UnauthorizedTaskAccessException(taskId);

        return task;
    }
}
