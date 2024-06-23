namespace TodoListApp.WebApp.Logging;

public static partial class TodoListLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "HTTP error occurred while getting paginated todo lists: {Message}")]
    public static partial void HTTPErrorWhileGettingLists(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Invalid operation occurred while fetching todo lists.: {Message}")]
    public static partial void IOEGettingTodoLists(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Error getting paginated todo lists: {Message}")]
    public static partial void ErrorGettingTodoLists(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Invalid Operation Exception while deleting todo list with ID: {Id}. {Message}")]
    public static partial void IOEWhileDeleting(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Error deleting todo list with ID: {Id}. {Message}")]
    public static partial void ErrorDeletingTodoList(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Invalid Operation occurred while adding new todo list: {Message}")]
    public static partial void IOErrorWhileAddingList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "Error adding new todo list: {Message}")]
    public static partial void ErrorAddingTodoList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "Invalid Operation error occurred while getting todo list details for ID : {Id}. {Message}")]
    public static partial void IOErrorGettingTodo(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "Error getting todo list details for ID: {Id}. {Message}")]
    public static partial void ErrorGettingTodoList(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "Invalid Operation occurred while fetching todo list with ID for an update: {Id}. {Message}")]
    public static partial void IOErrorFetchingTodoForUpdate(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 11, Level = LogLevel.Error, Message = "Error fetching todo list with ID for an update: {Id}. {Message}")]
    public static partial void ErrorFetchingTodoForUpdate(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 12, Level = LogLevel.Error, Message = "Invalid Operation occurred while updating todo list with ID: {Id}. {Message}")]
    public static partial void IOErrorUpdatingTodo(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 13, Level = LogLevel.Error, Message = "Error while updating todo list with ID: {Id}. {Message}")]
    public static partial void ErrorUpdatingTodoList(this ILogger logger, int id, string message, Exception ex);
}
