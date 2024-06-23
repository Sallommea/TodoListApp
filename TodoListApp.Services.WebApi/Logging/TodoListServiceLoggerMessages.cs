using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.WebApi.Logging;
public static partial class TodoListServiceLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "HTTP error occurred while getting paginated todo lists: {Message}")]
    public static partial void HTTPErrorWhileGettingLists(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error getting paginated todo lists: {Message}")]
    public static partial void ErrorGettingTodoLists(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "HTTP error occurred while deleting todo list with ID: {Id}. {Message}")]
    public static partial void HTTPErrorWhileDeleting(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Error deleting todo list with ID: {Id}. {Message}")]
    public static partial void ErrorDeletingTodoList(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "HTTP error occurred while adding new todo list: {Message}")]
    public static partial void HTTPErrorWhileAddingList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Error adding new todo list: {Message}")]
    public static partial void ErrorAddingTodoList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "HTTP error occurred while getting todo list details for ID : {Id}. {Message}")]
    public static partial void HTTPErrorGettingTodo(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "Error getting todo list details for ID: {Id}. {Message}")]
    public static partial void ErrorGettingTodoList(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "HTTP error occurred while updating todo list with ID: {Id}. {Message}")]
    public static partial void HTTPErrorUpdatingTodo(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "Error updating todo list with ID: {Id}. {Message}")]
    public static partial void ErrorUpdatingTodoList(this ILogger logger, int id, string message, Exception ex);
}
