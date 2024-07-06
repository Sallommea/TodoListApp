using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.WebApi.Logging;
public static partial class AuthServiceLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "HTTP An error occurred signing in: {Message}")]
    public static partial void HTTPErrorWhileSigninIn(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while signing in: {Message}")]
    public static partial void ErrorSigninIn(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "HTTP An error occurred while registering an user: {Message}")]
    public static partial void HTTPErrorWhileRegistering(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "An unexpected error occurred while registering an user: {Message}")]
    public static partial void ErrorWhileRegistering(this ILogger logger, string message, Exception ex);
}
