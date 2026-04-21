using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Core.Enums;

namespace TaskManagementSystem.Core.ViewModels;

public class TaskEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = "Priority is required.")]
    [Display(Name = "Priority")]
    public Priority Priority { get; set; }

    [Display(Name = "Mark as Completed")]
    public bool IsCompleted { get; set; }
}
