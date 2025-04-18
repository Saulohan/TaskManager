using Microsoft.EntityFrameworkCore;
using TaskManager.Database;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public partial class TaskItemHistoricRepository(TaskManagerContext context)
{
    public async Task Save(TaskItemHistoric taskItemHistoric)
    {
        if (taskItemHistoric.Id is null)
            await AddtaskItemHistoric(taskItemHistoric);
        else
            await UpdatetaskItemHistoric(taskItemHistoric);

        await context.SaveChangesAsync();
    }

    public async Task AddtaskItemHistoric(TaskItemHistoric taskItemHistoric)
    {
        await context.TaskItemHistoric.AddAsync(taskItemHistoric);
    } 

    public async Task UpdatetaskItemHistoric(TaskItemHistoric taskItemHistoric)
    {
        context.TaskItemHistoric.Update(taskItemHistoric);
    }
}