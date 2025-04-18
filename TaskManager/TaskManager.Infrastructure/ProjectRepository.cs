using Microsoft.EntityFrameworkCore;
using TaskManager.Database;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public partial class ProjectRepository(TaskManagerContext context)
{

    public async Task Save(Project project)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project), "O projeto não pode ser nulo.");
        }

        if (project.Id is null)
            await AddProject(project);
        else
            await UpdateProject(project);

        // validar necessidade
        await context.SaveChangesAsync();
    }


    public async Task AddProject(Project project)
    {
        await context.Project.AddAsync(project);
    } 

    public async Task UpdateProject(Project project)
    {
        context.Project.Update(project);
    }

    public async Task DeleteProject(Project project)
    {
        await context.SaveChangesAsync();
    }
    public async Task<List<Project>> GetAllProjects() => await context.Project.Where(x => x.DeletedAt == null).ToListAsync();

    public async Task<Project> GetProjectById(long projectId) => await context.Project.Where(x => x.DeletedAt == null && x.Id == projectId).FirstOrDefaultAsync();

}