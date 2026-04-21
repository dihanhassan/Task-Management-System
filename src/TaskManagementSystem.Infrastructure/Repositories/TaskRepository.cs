using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Enums;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Infrastructure.Data;

namespace TaskManagementSystem.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetByUserAsync(
        string userId, string searchTerm, TaskFilter filter, TaskSort sort)
    {
        var query = _context.Tasks.Where(t => t.UserId == userId);
        query = ApplySearch(query, searchTerm);
        query = ApplyFilter(query, filter);
        query = ApplySort(query, sort);
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteAsync(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ToggleStatusAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is null) return false;

        task.IsCompleted = !task.IsCompleted;
        await _context.SaveChangesAsync();
        return task.IsCompleted;
    }

    public async Task<(int Total, int Completed, int Pending, int Overdue)> GetCountsByUserAsync(string userId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.UserId == userId)
            .AsNoTracking()
            .Select(t => new { t.IsCompleted, t.DueDate })
            .ToListAsync();

        var total = tasks.Count;
        var completed = tasks.Count(t => t.IsCompleted);
        var pending = tasks.Count(t => !t.IsCompleted);
        var overdue = tasks.Count(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value.Date < DateTime.UtcNow.Date);

        return (total, completed, pending, overdue);
    }

    private static IQueryable<TaskItem> ApplySearch(IQueryable<TaskItem> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        return query.Where(t => t.Title.Contains(searchTerm));
    }

    private static IQueryable<TaskItem> ApplyFilter(IQueryable<TaskItem> query, TaskFilter filter)
    {
        return filter switch
        {
            TaskFilter.Completed => query.Where(t => t.IsCompleted),
            TaskFilter.Pending => query.Where(t => !t.IsCompleted),
            _ => query
        };
    }

    private static IQueryable<TaskItem> ApplySort(IQueryable<TaskItem> query, TaskSort sort)
    {
        return sort switch
        {
            TaskSort.DueDateDesc => query.OrderByDescending(t => t.DueDate),
            TaskSort.PriorityAsc => query.OrderBy(t => t.Priority),
            TaskSort.PriorityDesc => query.OrderByDescending(t => t.Priority),
            _ => query.OrderBy(t => t.DueDate)
        };
    }
}
