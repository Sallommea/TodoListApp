using Microsoft.EntityFrameworkCore;

namespace TodoListApp.Services.Database;
public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
           : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists { get; set; }

    public DbSet<TaskEntity> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure TodoListEntity
        _ = modelBuilder.Entity<TodoListEntity>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Description).HasMaxLength(500);

            // Configure the relationship with TaskEntity
            _ = entity.HasMany(e => e.Tasks)
                .WithOne(t => t.TodoList)
                .HasForeignKey(t => t.TodoListId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TaskEntity
        _ = modelBuilder.Entity<TaskEntity>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Description).HasMaxLength(500);
            _ = entity.Property(e => e.CreatedDate).IsRequired();
            _ = entity.Property(e => e.Status).IsRequired();
            _ = entity.Property(e => e.Assignee).IsRequired();

            // Configure the foreign key relationship with TodoListEntity
            _ = entity.HasOne(e => e.TodoList)
                .WithMany(t => t.Tasks)
                .HasForeignKey(e => e.TodoListId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
