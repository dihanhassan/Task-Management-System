using TaskManagementSystem.Core.Enums;
using X.PagedList;

namespace TaskManagementSystem.Core.ViewModels;

public class TaskListViewModel
{
    public IPagedList<TaskRowViewModel> Tasks { get; set; } = null!;
    public DashboardViewModel Dashboard { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public TaskFilter ActiveFilter { get; set; }
    public TaskSort ActiveSort { get; set; }
}
