using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Enums;

namespace TaskManagementSystem.Core.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetByUserAsync(string userId, string searchTerm, TaskFilter filter, TaskSort sort);
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    Task<bool> ToggleStatusAsync(int id);
    Task<(int Total, int Completed, int Pending, int Overdue)> GetCountsByUserAsync(string userId);
}
