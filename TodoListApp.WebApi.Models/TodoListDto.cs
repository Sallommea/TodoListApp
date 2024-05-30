using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models;
public class TodoListDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public int TaskCount { get; set; }

    public bool IsShared { get; set; }
}
