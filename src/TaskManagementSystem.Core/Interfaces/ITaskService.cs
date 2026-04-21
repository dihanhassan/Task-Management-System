using TaskManagementSystem.Core.Enums;
using TaskManagementSystem.Core.ViewModels;

namespace TaskManagementSystem.Core.Interfaces;

public interface ITaskService
{
    Task<TaskListViewModel> GetTaskListAsync(string userId, string searchTerm, TaskFilter filter, TaskSort sort, int page, int pageSize);
    Task<TaskEditViewModel> GetForEditAsync(int taskId, string userId);
    Task CreateAsync(TaskCreateViewModel model, string userId);
    Task UpdateAsync(TaskEditViewModel model, string userId);
    Task DeleteAsync(int taskId, string userId);
    Task<bool> ToggleStatusAsync(int taskId, string userId);
    Task<byte[]> ExportToCsvAsync(string userId);
}
