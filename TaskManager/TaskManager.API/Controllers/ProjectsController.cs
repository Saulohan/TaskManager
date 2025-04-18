using TaskManager.Database;
using TaskManager.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Domain.DTOs;
using TaskManager.Domain.Mappers;
using TaskManagerAPI.Utils;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly TaskManagerContext _context;
        private readonly ProjectRepository _projectRepository;
        private readonly TaskItemRepository _taskItemRepository;

        public ProjectsController(TaskManagerContext context, ProjectRepository projectRepository, TaskItemRepository taskItemRepository)
        {
            _context = context;
            _projectRepository = projectRepository;
            _taskItemRepository = taskItemRepository;
        }

        //// GET: Users
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{

        //    List<User> AllUsers = await _userRepository.GetAllUsers();

        //    if (AllUsers.Count == 0)
        //        return NotFound();

        //    return Ok(AllUsers.ToDTO());
        //}

        ////GET: Users/5
        //[HttpGet("id")]
        //public async Task<IActionResult> Get(long? id)
        //{
        //    if (id == null || id == 0)
        //        return NotFound("User Not Found");

        //    User user = await _userRepository.GetUserById((long)id);

        //    if (user == null)
        //        return NotFound("User Not Found");

        //    UserDTO userDTO = user.ToDTO();

        //    return Ok(userDTO);
        //}


        //[HttpPost("Create")]
        //public async Task<IActionResult> Create([FromServices] IPasswordHasher<User> hasher, [FromBody] CreateUserDTO createUserDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    string validationResult = await new UserValidator(_userRepository).ValidateUser(createUserDTO);

        //    if (!string.IsNullOrEmpty(validationResult))
        //        return BadRequest(validationResult);

        //    string hashedPassword = hasher.HashPassword(null, createUserDTO.Password);

        //    User user = new()
        //    {
        //        Username = createUserDTO.Name,
        //        PasswordHash = hashedPassword,
        //        Status = UserStatus.Enabled,
        //        Type = UserType.Client,
        //        CreatedAt = DateTime.Now,
        //        UpdatedAt = DateTime.Now,
        //        Person = new Person
        //        {
        //            Name = createUserDTO.Name,
        //            Phone = createUserDTO.Phone,
        //            Email = createUserDTO.Email,
        //            CreatedAt = DateTime.Now,
        //            UpdatedAt = DateTime.Now,
        //        }
        //    };

        //    _context.User.Add(user);

        //    try
        //    {
        //        await _userRepository.AddAsync(user);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }

        //    return Created($"API/Users/{user.Id}", user.ToDTO());
        //}

        //[HttpPost("Login")]
        //public async Task<IActionResult> Login([FromServices] IPasswordHasher<User> hasher, [FromBody] LoginUserDTO loginUserDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    User user = await _userRepository.GetUserByEmail(loginUserDTO.Email);

        //    if (user == null)
        //        return NotFound("Usuário inválido");

        //    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDTO.Password);

        //    if (result == PasswordVerificationResult.Failed)
        //        return Unauthorized("Senha incorreta");

        //    return Ok();
        //}



        // GET /projects
        [HttpGet("GetAllProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                List<Project> projects = await _projectRepository.GetAllProjects();

                if (projects is null || !projects.Any())
                    return Ok(new { Sucess = false, message = "Nenhum projeto encontrado.", projects = new List<Project>() });         

                return Ok(new { Sucess = false, message = "Projetos recuperados com sucesso.", projects });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex);  // ver se vai ficar ai mesmo.
                return StatusCode(500, new { Sucess = false, mensagem = $"Houve um erro no sistema: {ex.Message}" });
            }
        }

        [HttpPost("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDTO createProjectDTO)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createProjectDTO.Title))
                    return BadRequest(new { Sucess = false, mensagem = "Título do projeto é obrigatório." });

                Project project = ProjectMapper.CreateProjectDTOToProduct(createProjectDTO);

                await _projectRepository.Save(project);

                return Ok(new
                {
                    Sucess = true,
                    mensagem = "Projeto criado com sucesso."
                });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex); // ver se vai ficar ai mesmo.
                return StatusCode(500, new { Sucess = false, mensagem = $"Houve um erro no sistema: {ex.Message}" });
            }
        }

        [HttpPost("DeleteProject")]
        public async Task<IActionResult> DeleteProject(long projectId)
        {
            try
            {
                Project project = await _projectRepository.GetProjectById(projectId);

                if (project is null)
                    return NotFound(new { Success = false, message = "Projeto não encontrado." });

                List<TaskItem> taskItems = await _taskItemRepository.GetAllTasksByProjectId(projectId);

                if(taskItems.Any() || taskItems is null)//validar qual usar
                    return NotFound(new { Success = false, message = "Não é possivel remover um projeto com tarefas ativas, finalize as tarefas pendentes antes da remoção." });

                project.DeletedAt = DateTimeOffset.UtcNow;
                project.UpdatedBy = 0;// adicionar o this

                await _projectRepository.DeleteProject(project);

                return Ok(new { Success = true, message = "Projeto excluído com sucesso." });
            }
            catch (Exception ex)
            {
                Utils.SaveLogError(ex); // ver se vai ficar ai mesmo.
                return StatusCode(500, new { Success = false, message = $"Erro ao tentar excluir o projeto: {ex.Message}" });
            }
        }

    }
}