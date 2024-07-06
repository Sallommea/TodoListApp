namespace TodoListApp.WebApi.Logging;

public static partial class LoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Fetching paginated todo lists, PageNumber: {PageNumber}, ItemsPerPage: {ItemsPerPage}")]
    public static partial void FetchingPaginatedTodoLists(this ILogger logger, int pageNumber, int itemsPerPage);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Successfully fetched paginated todo lists.")]
    public static partial void SuccessfullyFetchedPaginatedTodoLists(this ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "TodoListException occurred while fetching paginated todo lists.")]
    public static partial void TodoListExceptionOccurred(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "An unexpected error occurred while fetching paginated todo lists.")]
    public static partial void UnexpectedErrorOccurred(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "A database error occurred while adding the todo list.")]
    public static partial void DatabaseErrorOccurred(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "An unexpected error occurred while adding the todo list.")]
    public static partial void UnexpectedErrorOccurredWhileAddingTodoList(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "An unexpected error occurred while updating the todo list.")]
    public static partial void UnexpectedErrorOccurredWhileUpdatingTodoList(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "An unexpected error occurred while deleting the todo list.")]
    public static partial void UnexpectedErrorOccurredWhileDeletingTodoList(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "TodoListException occurred while adding the todo list: {Message}")]
    public static partial void TodoListExceptionOccurredWhileAddingTodoList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "TodoListException occurred while updating the todo list: {Message}")]
    public static partial void TodoListExceptionOccurredWhileUpdatingTodoList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 11, Level = LogLevel.Error, Message = "TodoListException occurred while deleting the todo list: {Message}")]
    public static partial void TodoListExceptionOccurredWhileDeletingTodoList(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 12, Level = LogLevel.Information, Message = "Todo list added. Name: {Name}")]
    public static partial void TodoListAdded(this ILogger logger, string name);

    [LoggerMessage(EventId = 13, Level = LogLevel.Information, Message = "Todo list updated. ID: {Id}, Name: {Name}")]
    public static partial void TodoListUpdated(this ILogger logger, int id, string name);

    [LoggerMessage(EventId = 14, Level = LogLevel.Information, Message = "Todo list deleted. ID: {Id}")]
    public static partial void TodoListDeleted(this ILogger logger, int id);

    [LoggerMessage(EventId = 15, Level = LogLevel.Warning, Message = "Todo list with ID {TodoListId} not found.")]
    public static partial void TodoListNotFound(this ILogger logger, int todoListId);

    [LoggerMessage(EventId = 16, Level = LogLevel.Warning, Message = "Invalid page number: {PageNumber}")]
    public static partial void InvalidPageNumber(this ILogger logger, int pageNumber);

    [LoggerMessage(EventId = 17, Level = LogLevel.Warning, Message = "Invalid items per page: {ItemsPerPage}")]
    public static partial void InvalidItemsPerPage(this ILogger logger, int itemsPerPage);

    [LoggerMessage(EventId = 18, Level = LogLevel.Error, Message = "TodoListException occurred while fetching todo list details with tasks.")]
    public static partial void TodoListExceptionOccurredWhileGettingTodoDetails(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 19, Level = LogLevel.Error, Message = "An error occurred while retrieving the todo list with tasks.")]
    public static partial void UnexpectedErrorOccurredWhileGettingTodoDetails(this ILogger logger, Exception ex);

    [LoggerMessage(EventId = 20, Level = LogLevel.Error, Message = "Invalid Operation occured while retrieving the todo lists: {Message}")]
    public static partial void InvalidOperationOccurredWhileGettingTodoLists(this ILogger logger, string message, Exception ex);
}
