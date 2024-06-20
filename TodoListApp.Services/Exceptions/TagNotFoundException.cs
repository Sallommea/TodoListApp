namespace TodoListApp.Services.Exceptions;
public class TagNotFoundException : Exception
{
    public TagNotFoundException(int tagId)
        : base($"Tag with ID {tagId} was not found.")
    {
    }

    public TagNotFoundException()
    {
    }

    public TagNotFoundException(string message)
        : base(message)
    {
    }

    public TagNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
