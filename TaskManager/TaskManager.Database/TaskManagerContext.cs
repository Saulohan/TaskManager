using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Database;

public partial class TaskManagerContext : DbContext
{
    public TaskManagerContext(DbContextOptions<TaskManagerContext> options) : base(options) { }

    public DbSet<User> User { get; protected set; }

    public DbSet<Project> Project { get; set; }

    public DbSet<TaskItem> TaskItem { get; set; }

    public DbSet<TaskItemHistoric> TaskItemHistoric { get; set; }

    public DbSet<TaskComment> TaskComment { get; set; }
}