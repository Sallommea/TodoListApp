namespace TodoListApp.Services.Exceptions;
public class TodoListException : Exception
{
    public TodoListException(string? message)
        : base(message)
    {
    }

    public TodoListException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public TodoListException()
    {
    }
}
