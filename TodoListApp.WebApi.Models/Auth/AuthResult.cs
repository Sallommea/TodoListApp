namespace TodoListApp.WebApi.Models.Auth;
public class AuthResult
{
    public bool Success { get; set; }

    public string Token { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
