using System.Text;
using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Infrastructure.Helpers;

public static class CsvExportHelper
{
    private const string CsvHeader = "Id,Title,Description,DueDate,Priority,IsCompleted,CreatedAt";

    public static byte[] GenerateCsv(IEnumerable<TaskItem> tasks)
    {
        var sb = new StringBuilder();
        sb.AppendLine(CsvHeader);

        foreach (var task in tasks)
        {
            sb.AppendLine(FormatRow(task));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string FormatRow(TaskItem task)
    {
        return string.Join(",",
            task.Id,
            Escape(task.Title),
            Escape(task.Description ?? string.Empty),
            task.DueDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            task.Priority.ToString(),
            task.IsCompleted,
            task.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        );
    }

    private static string Escape(string value)
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
