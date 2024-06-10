using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models.Tasks;
public class UpdateTaskDto : IValidatableObject
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public Status Status { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (this.DueDate.HasValue && this.DueDate.Value < DateTime.UtcNow)
        {
            yield return new ValidationResult("DueDate cannot be in the past.", new[] { nameof(this.DueDate) });
        }
    }
}
