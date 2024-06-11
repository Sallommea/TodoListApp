using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.Logging;
public static partial class TodoListLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Todo list with ID {TodoListId} not found: {Message}")]
    public static partial void TodoListNotFound(this ILogger logger, int todoListId, string message);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while getting todolist with ID {TodoListId}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurred(this ILogger logger, string message, int todoListid, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Unexpected Error while retrieving todos: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileRetrievingTodos(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "An unexpected error occurred while adding the todo list: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileAddingTodoList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "An unexpected error occurred while updating the todo list.{Message}")]
    public static partial void UnexpectedErrorOccurredWhileUpdatingTodoList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "An unexpected error occurred while deleting the todo list. {Message}")]
    public static partial void UnexpectedErrorOccurredWhileDeletingTodoList(this ILogger logger, string message, Exception ex);
}
