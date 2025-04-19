using Microsoft.EntityFrameworkCore;
using TaskManager.Database;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure;

public partial class ProjectRepository(TaskManagerContext context)
{

    public virtual async Task Save(Project project)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project), "O projeto não pode ser nulo.");
        }

        if (project.Id is null)
            await AddProject(project);
        else
            await UpdateProject(project);

        await context.SaveChangesAsync();
    }


    public virtual async Task AddProject(Project project)
    {
        await context.Project.AddAsync(project);
    } 

    public virtual async Task UpdateProject(Project project)
    {
        context.Project.Update(project);
    }

    public virtual async Task DeleteProject(Project project)
    {
        await context.SaveChangesAsync();
    }
    public virtual async Task<List<Project>> GetAllProjects() => await context.Project.Include(p => p.User).Where(x => x.DeletedAt == null).ToListAsync();

    public virtual async Task<Project> GetProjectById(long projectId) => await context.Project .Include(p => p.User) .Where(x => x.DeletedAt == null && x.Id == projectId) .FirstOrDefaultAsync();
}