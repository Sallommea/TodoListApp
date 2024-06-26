using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.Logging;
public static partial class TaskLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Task with ID {TaskId} created.")]
    public static partial void TaskCreated(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while creating a task.")]
    public static partial void UnexpectedErrorCreatingTask(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "An error occurred while retrieving the task details. {Message}")]
    public static partial void UnexpectedErrorOccurredWhileGettingTaskDetails(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Task with ID {TaskId} not found to get task details")]
    public static partial void TaskIdNotFoundToGetTaskDetails(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Todo List not found: {Message}")]
    public static partial void ToDoListNotFound(this ILogger logger, string message);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Task with ID {TaskId} not found to delete task.")]
    public static partial void TaskIdNotFoundToDelete(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Task with ID {TaskId} deleted successfully.")]
    public static partial void TaskDeletedSuccessfully(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "An unexpected error occurred while deleting task with ID {TaskId}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileDeletingTask(this ILogger logger, string message, int taskId, Exception ex);

    [LoggerMessage(EventId = 9, Level = LogLevel.Warning, Message = "Task ID not found for update: {TaskId}")]
    public static partial void TaskIdNotFoundForUpdate(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "An unexpected error occurred while updating task with ID {TaskId}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileUpdatingTask(this ILogger logger, string message, int taskId, Exception ex);

    [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "Serviece retrieved searched tasks successfully.")]
    public static partial void SearchedTasksRetrieved(this ILogger logger);

    [LoggerMessage(EventId = 13, Level = LogLevel.Error, Message = "An unexpected error occurred while retrieving task searched with {SearchText}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileSearchingTasks(this ILogger logger, string message, string searchText, Exception ex);

    [LoggerMessage(EventId = 14, Level = LogLevel.Error, Message = "An unexpected error occurred while retrieving tasks by Tag Id: {TagId}. Error: {Message}")]
    public static partial void UnexpectedErrorgGettingTasksbyTagId(this ILogger logger, int tagId, string message, Exception ex);

    [LoggerMessage(EventId = 16, Level = LogLevel.Information, Message = "Tasks Retrieved by Tag Id:  {TagId}.")]
    public static partial void TasksByTagIdRetrieved(this ILogger logger, int tagId);

    [LoggerMessage(EventId = 17, Level = LogLevel.Warning, Message = "Task ID not found for adding comment: {TaskId}")]
    public static partial void TaskIdNotFoundForAddingComment(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 18, Level = LogLevel.Error, Message = "An unexpected error occurred while adding the comment. Error: {Message}")]
    public static partial void UnexpectedErrorAddingCommentOnTask(this ILogger logger, string message, Exception ex);
}
