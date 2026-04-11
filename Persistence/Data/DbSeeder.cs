using Domain.Models;
using Persistence.DataContext;

namespace Persistence.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationContext context, TimeProvider timeProvider)
    {
        if (context.Employees.Any())
            return;

        var now = timeProvider.GetUtcNow();
        var today = DateOnly.FromDateTime(now.DateTime);

        // ==============================
        // 1. СОТРУДНИКИ (The Artists)
        // ==============================

        // --- DIRECTOR ---
        var director = new Employee
        {
            FirstName = "Yuu",
            LastName = "Miyashita",
            MiddleName = "Utaite",
            Email = "yuu@villain.com",
            Role = Role.Director,
        };

        // --- MANAGERS ---
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

        var managerDeco = new Employee
        {
            FirstName = "Deco",
            LastName = "Twenty-Seven",
            MiddleName = "Streaming",
            Email = "deco27@android.girl",
            Role = Role.Manager,
        };

        // --- WORKERS ---
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

        var workerRin = new Employee
        {
            FirstName = "Rin",
            LastName = "Kagamine",
            MiddleName = "Meltdown",
            Email = "rin@lost.ones.weeping",
            Role = Role.Worker,
        };

        var workerLen = new Employee
        {
            FirstName = "Len",
            LastName = "Kagamine",
            MiddleName = "Paradichlorobenzene",
            Email = "len@servant.evil",
            Role = Role.Worker,
        };

        var workerLuka = new Employee
        {
            FirstName = "Megurine",
            LastName = "Luka",
            MiddleName = "Tarantula",
            Email = "luka@just.be.friends",
            Role = Role.Worker,
        };

        var workerMiku = new Employee
        {
            FirstName = "Hatsune",
            LastName = "Miku",
            MiddleName = "Disappearance",
            Email = "miku@deep.sea.girl",
            Role = Role.Worker,
        };

        var workerIa = new Employee
        {
            FirstName = "Aria",
            LastName = "IA",
            Email = "ia@imaginative.landscape",
            Role = Role.Worker,
        };

        var workerUna = new Employee
        {
            FirstName = "Otomachi",
            LastName = "Una",
            MiddleName = "Sugar",
            Email = "una@bitter.choco",
            Role = Role.Worker,
        };

        var employees = new List<Employee>
        {
            director,
            managerMaretu,
            managerKairiki,
            managerGhost,
            managerDeco,
            workerMili,
            workerFlower,
            workerKaai,
            workerGumi,
            workerRin,
            workerLen,
            workerLuka,
            workerMiku,
            workerIa,
            workerUna,
        };

        context.Employees.AddRange(employees);
        context.SaveChanges();

        // ==============================
        // 2. ПРОЕКТЫ (The Hallucinations)
        // ==============================

        var projectMaretu = new Project
        {
            Name = "Project: MIND BRAND REWIRING",
            Priority = 5,
            CompanyOrdering = "Brain Revolution Girl Corp",
            CompanyExecuting = "Coin Locker Baby Inc.",
            StartDate = today.AddDays(-10),
            EndDate = today.AddDays(30),
            ManagerId = managerMaretu.Id,
        };

        var projectKairiki = new Project
        {
            Name = "Operation: ALKALI UNDERWORLD",
            Priority = 3,
            CompanyOrdering = "Failure Girl LLC",
            CompanyExecuting = "Bug Eater Ltd.",
            StartDate = today.AddDays(-30),
            EndDate = today.AddDays(60),
            ManagerId = managerKairiki.Id,
        };

        var projectGhost = new Project
        {
            Name = "Initiative: HYPERDONTIA",
            Priority = 5,
            CompanyOrdering = "Tooth Fairy from Hell",
            CompanyExecuting = "Novocaine & Mirrors",
            StartDate = today,
            EndDate = today.AddDays(365),
            ManagerId = managerGhost.Id,
        };

        var projectDeco = new Project
        {
            Name = "Campaign: ANDROID AFFECTION",
            Priority = 2,
            CompanyOrdering = "Ai Kotoba Holdings",
            CompanyExecuting = "Streaming Heart Studios",
            StartDate = today.AddDays(-60),
            EndDate = today.AddDays(90),
            ManagerId = managerDeco.Id,
        };

        var projectNoManager = new Project
        {
            Name = "Archive: FORGOTTEN FREQUENCIES",
            Priority = 1,
            CompanyOrdering = "Obsolete Sound Corp",
            CompanyExecuting = null,
            StartDate = today.AddDays(-180),
            EndDate = null,
            ManagerId = null,
        };

        var projects = new List<Project>
        {
            projectMaretu,
            projectKairiki,
            projectGhost,
            projectDeco,
            projectNoManager,
        };

        context.Projects.AddRange(projects);
        context.SaveChanges();

        // ==============================
        // 3. УЧАСТНИКИ ПРОЕКТОВ (Members)
        // ==============================

        var members = new List<ProjectMember>
        {
            // Maretu's Project — chaos crew
            new() { ProjectId = projectMaretu.Id, EmployeeId = workerFlower.Id },
            new() { ProjectId = projectMaretu.Id, EmployeeId = workerGumi.Id },
            new() { ProjectId = projectMaretu.Id, EmployeeId = workerRin.Id },
            // Kairiki's Project — depression squad
            new() { ProjectId = projectKairiki.Id, EmployeeId = workerKaai.Id },
            new() { ProjectId = projectKairiki.Id, EmployeeId = workerMili.Id },
            new() { ProjectId = projectKairiki.Id, EmployeeId = workerLen.Id },
            // GHOST's Project — body horror division
            new() { ProjectId = projectGhost.Id, EmployeeId = workerFlower.Id },
            new() { ProjectId = projectGhost.Id, EmployeeId = workerMili.Id },
            new() { ProjectId = projectGhost.Id, EmployeeId = workerLuka.Id },
            new() { ProjectId = projectGhost.Id, EmployeeId = workerIa.Id },
            // Deco's Project — feelings department
            new() { ProjectId = projectDeco.Id, EmployeeId = workerMiku.Id },
            new() { ProjectId = projectDeco.Id, EmployeeId = workerGumi.Id },
            new() { ProjectId = projectDeco.Id, EmployeeId = workerUna.Id },
            new() { ProjectId = projectDeco.Id, EmployeeId = workerRin.Id },
            new() { ProjectId = projectDeco.Id, EmployeeId = workerLen.Id },
            // Forgotten project — lone survivor
            new() { ProjectId = projectNoManager.Id, EmployeeId = workerIa.Id },
        };

        context.ProjectMembers.AddRange(members);
        context.SaveChanges();

        // ==============================
        // 4. ЗАДАЧИ (The Madness)
        // ==============================

        var tasks = new List<WorkTask>
        {
            // ========== MARETU PROJECT ==========
            new()
            {
                Title = "Scream at the microwave until it explodes",
                Priority = 5,
                Status = WorkTaskStatus.Done,
                Comment = "Success. The kitchen is gone.",
                CreatedAt = now.AddDays(-5),
                UpdatedAt = now.AddDays(-3),
                AuthorId = managerMaretu.Id,
                ExecutorId = workerFlower.Id,
                ProjectId = projectMaretu.Id,
            },
            new()
            {
                Title = "Re-stitch the pinky swear",
                Priority = 3,
                Status = WorkTaskStatus.InProgress,
                Comment = "Need more thread. And sanity.",
                CreatedAt = now.AddDays(-2),
                AuthorId = managerMaretu.Id,
                ExecutorId = workerGumi.Id,
                ProjectId = projectMaretu.Id,
            },
            new()
            {
                Title = "Count every lie in the coin locker",
                Priority = 4,
                Status = WorkTaskStatus.ToDo,
                Comment = null,
                CreatedAt = now.AddDays(-1),
                AuthorId = managerMaretu.Id,
                ExecutorId = workerRin.Id,
                ProjectId = projectMaretu.Id,
            },
            new()
            {
                Title = "Calibrate the brain revolution frequency",
                Priority = 5,
                Status = WorkTaskStatus.InProgress,
                Comment = "Frequency keeps shifting. Might be alive.",
                CreatedAt = now.AddHours(-18),
                AuthorId = managerMaretu.Id,
                ExecutorId = workerFlower.Id,
                ProjectId = projectMaretu.Id,
            },
            // ========== KAIRIKI PROJECT ==========
            new()
            {
                Title = "Dance with the invisible Lemming",
                Priority = 2,
                Status = WorkTaskStatus.ToDo,
                Comment = "No motivation found.",
                CreatedAt = now,
                AuthorId = managerKairiki.Id,
                ExecutorId = workerKaai.Id,
                ProjectId = projectKairiki.Id,
            },
            new()
            {
                Title = "Drink the Venom (Metaphorically)",
                Priority = 5,
                Status = WorkTaskStatus.Done,
                Comment = "Tasted like purple.",
                CreatedAt = now.AddDays(-20),
                UpdatedAt = now.AddDays(-19),
                AuthorId = managerKairiki.Id,
                ExecutorId = workerMili.Id,
                ProjectId = projectKairiki.Id,
            },
            new()
            {
                Title = "Debug the Failure Girl algorithm",
                Priority = 3,
                Status = WorkTaskStatus.InProgress,
                Comment = "Algorithm keeps succeeding. That's the bug.",
                CreatedAt = now.AddDays(-7),
                UpdatedAt = now.AddDays(-2),
                AuthorId = managerKairiki.Id,
                ExecutorId = workerLen.Id,
                ProjectId = projectKairiki.Id,
            },
            new()
            {
                Title = "Feed the bug eater",
                Priority = 1,
                Status = WorkTaskStatus.ToDo,
                Comment = null,
                CreatedAt = now.AddDays(-3),
                AuthorId = managerKairiki.Id,
                ExecutorId = null,
                ProjectId = projectKairiki.Id,
            },
            // ========== GHOST PROJECT ==========
            new()
            {
                Title = "Pull out extra teeth from the jaw",
                Priority = 4,
                Status = WorkTaskStatus.InProgress,
                Comment = "Mirror cracked. Send help.",
                CreatedAt = now.AddHours(-5),
                AuthorId = managerGhost.Id,
                ExecutorId = workerFlower.Id,
                ProjectId = projectGhost.Id,
            },
            new()
            {
                Title = "Transcribe the honey distortion signal",
                Priority = 5,
                Status = WorkTaskStatus.ToDo,
                Comment = "Waiting for compile...",
                CreatedAt = now,
                AuthorId = managerGhost.Id,
                ExecutorId = workerMili.Id,
                ProjectId = projectGhost.Id,
            },
            new()
            {
                Title = "Reattach the phantom limbs",
                Priority = 3,
                Status = WorkTaskStatus.InProgress,
                Comment = "Left arm keeps phasing through walls.",
                CreatedAt = now.AddDays(-12),
                UpdatedAt = now.AddDays(-1),
                AuthorId = managerGhost.Id,
                ExecutorId = workerLuka.Id,
                ProjectId = projectGhost.Id,
            },
            new()
            {
                Title = "Map the communications breakdown",
                Priority = 2,
                Status = WorkTaskStatus.Done,
                Comment = "Turns out nobody was talking.",
                CreatedAt = now.AddDays(-30),
                UpdatedAt = now.AddDays(-25),
                AuthorId = managerGhost.Id,
                ExecutorId = workerIa.Id,
                ProjectId = projectGhost.Id,
            },
            new()
            {
                Title = "Honey I'm Home (But nobody is there)",
                Priority = 1,
                Status = WorkTaskStatus.ToDo,
                Comment = "Vivisect the spider.",
                CreatedAt = now.AddDays(-1),
                AuthorId = managerGhost.Id,
                ExecutorId = null,
                ProjectId = projectGhost.Id,
            },
            // ========== DECO PROJECT ==========
            new()
            {
                Title = "Compose the android's love letter",
                Priority = 3,
                Status = WorkTaskStatus.InProgress,
                Comment = "She keeps writing in binary.",
                CreatedAt = now.AddDays(-14),
                UpdatedAt = now.AddDays(-5),
                AuthorId = managerDeco.Id,
                ExecutorId = workerMiku.Id,
                ProjectId = projectDeco.Id,
            },
            new()
            {
                Title = "Stream the heart to all devices",
                Priority = 4,
                Status = WorkTaskStatus.ToDo,
                Comment = "Bandwidth insufficient for emotions.",
                CreatedAt = now.AddDays(-3),
                AuthorId = managerDeco.Id,
                ExecutorId = workerGumi.Id,
                ProjectId = projectDeco.Id,
            },
            new()
            {
                Title = "Translate Ai Kotoba to machine language",
                Priority = 2,
                Status = WorkTaskStatus.Done,
                Comment = "Output: 01001100 01001111 01010110 01000101",
                CreatedAt = now.AddDays(-45),
                UpdatedAt = now.AddDays(-40),
                AuthorId = managerDeco.Id,
                ExecutorId = workerUna.Id,
                ProjectId = projectDeco.Id,
            },
            new()
            {
                Title = "Deploy the ghost rule protocol",
                Priority = 5,
                Status = WorkTaskStatus.InProgress,
                Comment = "Protocol keeps haunting other services.",
                CreatedAt = now.AddDays(-8),
                UpdatedAt = now.AddHours(-6),
                AuthorId = managerDeco.Id,
                ExecutorId = workerRin.Id,
                ProjectId = projectDeco.Id,
            },
            new()
            {
                Title = "Synchronize the twin paradox",
                Priority = 3,
                Status = WorkTaskStatus.ToDo,
                Comment = "Rin says left. Len says right. Both are wrong.",
                CreatedAt = now.AddDays(-2),
                AuthorId = managerDeco.Id,
                ExecutorId = workerLen.Id,
                ProjectId = projectDeco.Id,
            },
            new()
            {
                Title = "Archive the two-breaths walking memory",
                Priority = 1,
                Status = WorkTaskStatus.ToDo,
                Comment = null,
                CreatedAt = now,
                AuthorId = managerDeco.Id,
                ExecutorId = null,
                ProjectId = projectDeco.Id,
            },
            // ========== FORGOTTEN PROJECT ==========
            new()
            {
                Title = "Listen to the silence between frequencies",
                Priority = 1,
                Status = WorkTaskStatus.InProgress,
                Comment = "Still listening. It's been 6 months.",
                CreatedAt = now.AddDays(-170),
                UpdatedAt = now.AddDays(-90),
                AuthorId = null,
                ExecutorId = workerIa.Id,
                ProjectId = projectNoManager.Id,
            },
            new()
            {
                Title = "Catalog the obsolete waveforms",
                Priority = 1,
                Status = WorkTaskStatus.ToDo,
                Comment = "Nobody remembers what these were for.",
                CreatedAt = now.AddDays(-150),
                AuthorId = null,
                ExecutorId = null,
                ProjectId = projectNoManager.Id,
            },
        };

        context.Tasks.AddRange(tasks);
        context.SaveChanges();
    }
}
