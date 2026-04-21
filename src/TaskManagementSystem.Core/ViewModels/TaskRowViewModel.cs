using TaskManagementSystem.Core.Enums;

namespace TaskManagementSystem.Core.ViewModels;

public class TaskRowViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsOverdue { get; set; }
}
