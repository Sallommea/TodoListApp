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

    public DbSet<TagEntity> Tags { get; set; }

    public DbSet<TaskTagEntity> TaskTags { get; set; }

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

            _ = entity.HasMany(t => t.TaskTags)
                .WithOne(tt => tt.Task)
                .HasForeignKey(tt => tt.TaskId)
                .IsRequired(false);
        });

        _ = modelBuilder.Entity<TagEntity>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Name).IsRequired().HasMaxLength(50);

            _ = entity.HasMany(t => t.TaskTags)
                .WithOne(tt => tt.Tag)
                .HasForeignKey(tt => tt.TagId)
                .IsRequired();
        });

        _ = modelBuilder.Entity<TaskTagEntity>(entity =>
        {
            _ = entity.HasKey(tt => new { tt.TaskId, tt.TagId });

            _ = entity.HasOne(tt => tt.Task)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = entity.HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
