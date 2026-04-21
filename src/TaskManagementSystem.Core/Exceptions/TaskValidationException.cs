namespace TaskManagementSystem.Core.Exceptions;

public class TaskValidationException : Exception
{
    public TaskValidationException(string message)
        : base(message)
    {
    }
}
