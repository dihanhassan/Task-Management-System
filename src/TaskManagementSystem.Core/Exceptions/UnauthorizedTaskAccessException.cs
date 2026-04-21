namespace TaskManagementSystem.Core.Exceptions;

public class UnauthorizedTaskAccessException : Exception
{
    public UnauthorizedTaskAccessException(int taskId)
        : base($"Access denied for task with ID {taskId}.")
    {
    }
}
