namespace TodoListApp.WebApi.Logging;

public static partial class AssignedTasksControllerLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Invalid page number: {PageNumber}")]
    public static partial void InvalidPageNumber(ILogger logger, int pageNumber);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Invalid tasksPerPage number: {TasksPerPage}")]
    public static partial void InvalidTasksPerPage(ILogger logger, int tasksPerPage);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "An error occurred while retrieving tasks assigned to the user: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileGettingUserTasks(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "Task with ID {TaskId} not found.")]
    public static partial void InvalidTaskIdForTaskStatusUpdate(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Task status updated successfully. TaskId: {TaskId}")]
    public static partial void TaskStatusUpdatedSuccessfully(ILogger logger, int taskId);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "An error occurred while updating task status: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileUpdatingTaskStatus(this ILogger logger, string message, Exception ex);
}
