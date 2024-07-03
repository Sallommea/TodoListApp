using Microsoft.AspNetCore.Identity;

namespace TodoListApp.Services.Database;
public class User : IdentityUser
{
    public string Firstname { get; set; } = string.Empty;

    public string Lastname { get; set; } = string.Empty;

    public ICollection<TodoListEntity>? Todos { get; set; }
}
