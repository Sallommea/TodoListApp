using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.Logging;
public static partial class TodoListLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Todo list not found.")]
    public static partial void TodoListNotFound(ILogger logger, Exception ex);
}
