using Microsoft.EntityFrameworkCore;
using TaskManager.Database;
using TaskManager.Domain.Entities;
using TaskManager.Models.Enums;

namespace TaskManager.Infrastructure;

public partial class TaskItemRepository(TaskManagerContext context)
{
    public virtual async Task Save(TaskItem taskItem)
    {
        if (taskItem == null)
        {
            throw new ArgumentNullException(nameof(taskItem), "A tarefa não pode ser nula.");
        }

        if (taskItem.Id is null)
            await AddTaskItem(taskItem);
        else
            await UpdateTaskItem(taskItem);

        await context.SaveChangesAsync();
    }


    public virtual async Task AddTaskItem(TaskItem taskItem)
    {
        await context.TaskItem.AddAsync(taskItem);
    } 

    public virtual async Task UpdateTaskItem(TaskItem taskItem)
    {
        context.TaskItem.Update(taskItem);
    }

    public virtual async Task DeleteTaskItem(TaskItem taskItem)
    {
        await context.SaveChangesAsync();
    }
    // fazer condicional de statatus
    public virtual async Task<List<TaskItem>> GetAllTasksByProjectId(long projectId) => await context.TaskItem.Include(p => p.Project).Where(x => x.DeletedAt == null && x.Project.Id == projectId).ToListAsync();
    
    public virtual async Task<TaskItem> GetTaskItemById(long taskItemId) => await context.TaskItem.Include(p => p.Project).Where(x => x.DeletedAt == null && x.Id == taskItemId).FirstOrDefaultAsync();

    public virtual async Task<int> CountTaskItemsByProjectId(long projectId) => await context.TaskItem.Where(x => x.DeletedAt == null &&  x.Project.Id == projectId).CountAsync();

    public virtual async Task<List<TaskItem>> GetCompletedTasksByUserAndDateRange(long userId, DateTime startDate) =>
        await context.TaskItem
            .Include(t => t.Project)
            .ThenInclude(p => p.User)
            .Where(t =>
                t.DeletedAt == null &&
                t.Project.User.Id == userId &&
                t.Status == TaskItemStatus.Completed &&
                t.UpdatedAt >= startDate
            )
            .ToListAsync();
}