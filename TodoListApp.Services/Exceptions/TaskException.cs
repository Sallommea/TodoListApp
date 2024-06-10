namespace TodoListApp.Services.Exceptions;
public class TaskException : Exception
{
    public TaskException(string message)
        : base(message)
    {
    }

    public TaskException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public TaskException()
    {
    }
}
