using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.ViewModels;

namespace TaskManagementSystem.Infrastructure.Helpers;

public static class TaskMapper
{
    public static TaskRowViewModel ToRowViewModel(TaskItem task)
    {
        return new TaskRowViewModel
        {
            Id = task.Id,
            Title = task.Title,
            DueDate = task.DueDate,
            Priority = task.Priority,
            IsCompleted = task.IsCompleted,
            IsOverdue = task.IsOverdue
        };
    }

    public static TaskEditViewModel ToEditViewModel(TaskItem task)
    {
        return new TaskEditViewModel
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Priority = task.Priority,
            IsCompleted = task.IsCompleted
        };
    }

    public static TaskItem ToEntity(TaskCreateViewModel model, string userId)
    {
        return new TaskItem
        {
            Title = model.Title,
            Description = model.Description,
            DueDate = model.DueDate,
            Priority = model.Priority,
            IsCompleted = model.IsCompleted,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
    }

    public static void ApplyEdit(TaskItem task, TaskEditViewModel model)
    {
        task.Title = model.Title;
        task.Description = model.Description;
        task.DueDate = model.DueDate;
        task.Priority = model.Priority;
        task.IsCompleted = model.IsCompleted;
    }

    public static DashboardViewModel ToDashboardViewModel(
        (int Total, int Completed, int Pending, int Overdue) counts)
    {
        return new DashboardViewModel
        {
            TotalTasks = counts.Total,
            CompletedTasks = counts.Completed,
            PendingTasks = counts.Pending,
            OverdueTasks = counts.Overdue
        };
    }
}
