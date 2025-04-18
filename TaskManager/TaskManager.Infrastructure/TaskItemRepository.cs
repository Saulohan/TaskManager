using Microsoft.EntityFrameworkCore;
using TaskManager.Database;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public partial class TaskItemRepository(TaskManagerContext context)
{
    public async Task Save(TaskItem taskItem)
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


    public async Task AddTaskItem(TaskItem taskItem)
    {
        await context.TaskItem.AddAsync(taskItem);
    } 

    public async Task UpdateTaskItem(TaskItem taskItem)
    {
        context.TaskItem.Update(taskItem);
    }

    public async Task DeleteTaskItem(TaskItem taskItem)
    {
        await context.SaveChangesAsync();
    }
    // fazer condicional de statatus
    public async Task<List<TaskItem>> GetAllTasksByProjectId(long projectId) => await context.TaskItem.Where(x => x.DeletedAt == null && x.Project.Id == projectId).ToListAsync();
    
    public async Task<TaskItem> GetTaskItemById(long taskItemId) => await context.TaskItem.Where(x => x.DeletedAt == null && x.Id == taskItemId).FirstOrDefaultAsync();

}