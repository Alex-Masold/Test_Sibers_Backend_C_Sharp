using Domain.Models;
using Persistence.DataContext;

namespace Persistence.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationContext context)
    {
        // Если данные уже есть — не сидим заново
        if (context.Employees.Any())
        {
            return;
        }

        // ==============================
        // 1. СОТРУДНИКИ (The Artists)
        // ==============================

        // --- DIRECTOR ---
        var director = new Employee()
        {
            FirstName = "Yuu",
            LastName = "Miyashita",
            MiddleName = "Utaite",
            Email = "yuu@villain.com",
            Role = Role.Director,
        };

        // --- MANAGERS (3 шт) ---
        var managerMaretu = new Employee
        {
            FirstName = "Goku",
            LastName = "Maretu",
            MiddleName = "MindBrand",
            Email = "maretu@white.happy",
            Role = Role.Manager,
        };

        var managerKairiki = new Employee
        {
            FirstName = "Bear",
            LastName = "Kairiki",
            MiddleName = "Venom",
            Email = "kairiki@lemming.ming",
            Role = Role.Manager,
        };

        var managerGhost = new Employee
        {
            FirstName = "GHOST",
            LastName = "Communications",
            MiddleName = "Distortion",
            Email = "ghost@honey.home",
            Role = Role.Manager,
        };

        // --- WORKERS (Несколько) ---
        var workerMili = new Employee
        {
            FirstName = "Cassie",
            LastName = "Mili",
            MiddleName = "Witch",
            Email = "world@execute.me",
            Role = Role.Worker,
        };

        var workerFlower = new Employee
        {
            FirstName = "V4",
            LastName = "Flower",
            Email = "flower@appetite.pleaser",
            Role = Role.Worker,
        };

        var workerKaai = new Employee
        {
            FirstName = "Yuki",
            LastName = "Kaai",
            MiddleName = "Lagtrain",
            Email = "lost@umbrella.corp",
            Role = Role.Worker,
        };

        var workerGumi = new Employee
        {
            FirstName = "Megpoid",
            LastName = "Gumi",
            Email = "copycat@circus.p",
            Role = Role.Worker,
        };

        var employees = new List<Employee>
        {
            director,
            managerMaretu,
            managerKairiki,
            managerGhost,
            workerMili,
            workerFlower,
            workerKaai,
            workerGumi,
        };

        context.Employees.AddRange(employees);
        context.SaveChanges(); // Сохраняем, чтобы получить ID

        // ==============================
        // 2. ПРОЕКТЫ (The Hallucinations)
        // ==============================

        var projects = new List<Project>
        {
            // Проект 1: Управляет MARETU (Хаос и Глитчи)
            new Project
            {
                Name = "Project: MIND BRAND REWIRING",
                Priority = 100, // Критический
                CompanyOrdering = "Brain Revolution Girl Corp",
                CompanyExecuting = "Coin Locker Baby Inc.",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                ManagerId = managerMaretu.Id,
            },
            // Проект 2: Управляет Kairiki Bear (Депрессия и Энергия)
            new Project
            {
                Name = "Operation: ALKALI UNDERWORLD",
                Priority = 5,
                CompanyOrdering = "Failure Girl LLC",
                CompanyExecuting = "Bug Eater Ltd.",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(2)),
                ManagerId = managerKairiki.Id,
            },
            // Проект 3: Управляет GHOST (Искажения тела и разума)
            new Project
            {
                Name = "Initiative: HYPERDONTIA",
                Priority = 666,
                CompanyOrdering = "Tooth Fairy from Hell",
                CompanyExecuting = "Novocaine & Mirrors",
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
                ManagerId = managerGhost.Id,
            },
        };

        context.Projects.AddRange(projects);
        context.SaveChanges();

        // ==============================
        // 3. УЧАСТНИКИ ПРОЕКТОВ (Members)
        // ==============================

        var members = new List<ProjectMember>
        {
            // Maretu's Project: Там работают Flower и Gumi
            new ProjectMember { ProjectId = projects[0].Id, EmployeeId = workerFlower.Id },
            new ProjectMember { ProjectId = projects[0].Id, EmployeeId = workerGumi.Id },
            // Kairiki's Project: Там работают Kaai Yuki и Mili
            new ProjectMember { ProjectId = projects[1].Id, EmployeeId = workerKaai.Id },
            new ProjectMember { ProjectId = projects[1].Id, EmployeeId = workerMili.Id },
            // GHOST's Project: Все страдают
            new ProjectMember { ProjectId = projects[2].Id, EmployeeId = workerFlower.Id },
            new ProjectMember { ProjectId = projects[2].Id, EmployeeId = workerMili.Id },
        };

        context.ProjectMembers.AddRange(members);
        context.SaveChanges();

        // ==============================
        // 4. ЗАДАЧИ (The Madness)
        // ==============================

        var tasks = new List<WorkTask>
        {
            // --- MARETU PROJECT TASKS ---
            new WorkTask
            {
                Title = "Scream at the microwave until it explodes",
                Priority = 10,
                Status = WorkTaskStatus.Done,
                Comment = "Success. The kitchen is gone.",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                AuthorId = managerMaretu.Id,
                ExecutorId = workerFlower.Id,
                ProjectId = projects[0].Id,
            },
            new WorkTask
            {
                Title = "Re-stitch the pinky swear",
                Priority = 5,
                Status = WorkTaskStatus.InProgress,
                Comment = "Need more thread.",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                AuthorId = managerMaretu.Id,
                ExecutorId = workerGumi.Id,
                ProjectId = projects[0].Id,
            },
            // --- KAIRIKI BEAR PROJECT TASKS ---
            new WorkTask
            {
                Title = "Dance with the invisible Lemming",
                Priority = 3,
                Status = WorkTaskStatus.ToDo,
                Comment = "No motivation found.",
                CreatedAt = DateTime.UtcNow,
                AuthorId = managerKairiki.Id,
                ExecutorId = workerKaai.Id,
                ProjectId = projects[1].Id,
            },
            new WorkTask
            {
                Title = "Drink the Venom (Metaphorically)",
                Priority = 99,
                Status = WorkTaskStatus.Done,
                Comment = "Tasted like purple.",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-19),
                AuthorId = managerKairiki.Id,
                ExecutorId = workerMili.Id,
                ProjectId = projects[1].Id,
            },
            // --- GHOST PROJECT TASKS ---
            new WorkTask
            {
                Title = "Pull out extra teeth from the jaw",
                Priority = 1,
                Status = WorkTaskStatus.InProgress,
                Comment = "Mirror cracked. send help.",
                CreatedAt = DateTime.UtcNow.AddHours(-5),
                AuthorId = managerGhost.Id,
                ExecutorId = workerFlower.Id,
                ProjectId = projects[2].Id,
            },
            new WorkTask
            {
                Title = "World.Execute(Me);",
                Priority = 1000,
                Status = WorkTaskStatus.ToDo,
                Comment = "Waiting for compile...",
                CreatedAt = DateTime.UtcNow,
                AuthorId = managerGhost.Id,
                ExecutorId = workerMili.Id,
                ProjectId = projects[2].Id,
            },
            // Задача без исполнителя (висяк)
            new WorkTask
            {
                Title = "Honey I'm Home (But nobody is there)",
                Priority = 2,
                Status = WorkTaskStatus.ToDo,
                Comment = "Vivisect the spider.",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                AuthorId = managerGhost.Id,
                ExecutorId = null, // Никто не хочет это делать
                ProjectId = projects[2].Id,
            },
        };

        context.Tasks.AddRange(tasks);
        context.SaveChanges();
    }
}
