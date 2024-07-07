namespace TodoListApp.WebApi.Logging;

public static partial class TaskControllerLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Task created successfully. TaskId: {TaskId}, TodoListId: {TodoListId}")]
    public static partial void TaskCreatedSuccessfully(this ILogger logger, int taskId, int todoListId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Todo list not found. Unable to create task.")]
    public static partial void TodoListNotFound(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "An unexpected error occurred while creating the task.")]
    public static partial void UnexpectedErrorCreatingTask(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "An error occurred while retrieving the task details. {Message}")]
    public static partial void UnexpectedErrorOccurredWhileGettingTaskDetails(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "TaskException occurred while getting task details: {Message}")]
    public static partial void TaskExceptionOccurredWhileGettingTaskDetails(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 6, Level = LogLevel.Warning, Message = "Invalid task ID provided for deletion: {TaskId}")]
    public static partial void InvalidTaskIdForDeletion(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Task with ID {TaskId} not found to delete task.")]
    public static partial void TaskIdNotFoundToDelete(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 8, Level = LogLevel.Information, Message = "Task with ID {TaskId} deleted successfully.")]
    public static partial void TaskDeletedSuccessfully(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "An unexpected error occurred while deleting task with ID {TaskId}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileDeletingTask(this ILogger logger, string message, int taskId, Exception ex);

    [LoggerMessage(EventId = 10, Level = LogLevel.Warning, Message = "Invalid task ID provided for update: {TaskId}")]
    public static partial void InvalidTaskIdForUpdate(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 11, Level = LogLevel.Error, Message = "An unexpected error occurred while updating task with ID {TaskId}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileUpdatingTask(this ILogger logger, string message, int taskId, Exception ex);

    [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "Task with ID {TaskId} not found to update task.")]
    public static partial void TaskIdNotFoundToUpdate(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 13, Level = LogLevel.Information, Message = "Serviece retrieved searched tasks successfully.")]
    public static partial void SearchedTasksRetrieved(this ILogger logger);

    [LoggerMessage(EventId = 14, Level = LogLevel.Error, Message = "An unexpected error occurred while retrieving task searched with {SearchText}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileSearchingTasks(this ILogger logger, string message, string searchText, Exception ex);

    [LoggerMessage(EventId = 15, Level = LogLevel.Error, Message = "An unexpected error occurred while retrieving tasks by Tag Id: {TagId}. Error: {Message}")]
    public static partial void UnexpectedErrorgGettingTasksbyTagId(this ILogger logger, int tagId, string message, Exception ex);

    [LoggerMessage(EventId = 16, Level = LogLevel.Error, Message = "An invalid operation exception occured while adding the comment. Error: {Message}")]
    public static partial void IOExceptionWhileAddingComment(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 17, Level = LogLevel.Error, Message = "An unexpected error occurred adding the comment. Error: {Message}")]
    public static partial void UnexpectedErrorWhileAddingComment(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 18, Level = LogLevel.Error, Message = "An invalid operation exception occured while editing the comment. Error: {Message}")]
    public static partial void IOExceptionWhileEditingComment(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 19, Level = LogLevel.Error, Message = "An unexpected error occurred editing the comment. Error: {Message}")]
    public static partial void UnexpectedErrorWhileEditingComment(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 20, Level = LogLevel.Error, Message = "An invalid operation exception occured while deleting the comment. Error: {Message}")]
    public static partial void IOExceptionWhileDeletingComment(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 21, Level = LogLevel.Error, Message = "An unexpected error occurred deleting the comment. Error: {Message}")]
    public static partial void UnexpectedErrorWhileDeletingComment(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 22, Level = LogLevel.Error, Message = "Invalid Operation occured while getting searched Tasks: {Message}")]
    public static partial void InvalidOperationOccurredGettingSearchedTasks(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 23, Level = LogLevel.Error, Message = "Invalid Operation occured while getting task details: {Message}")]
    public static partial void InvalidOperationOccurredGettingTaskDetails(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 24, Level = LogLevel.Error, Message = "Invalid Operation occured while getting tasks by tag: {Message}")]
    public static partial void InvalidOperationOccurredGettingTasksByTag(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 25, Level = LogLevel.Error, Message = "Invalid Operation occured while creating task: {Message}")]
    public static partial void InvalidOperationCreatingTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 26, Level = LogLevel.Error, Message = "Invalid Operation occured while deleting task: {Message}")]
    public static partial void InvalidOperationDeletingTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 27, Level = LogLevel.Error, Message = "Invalid Operation occured while updating task: {Message}")]
    public static partial void InvalidOperationUpdatingTask(this ILogger logger, string message, Exception ex);
}
