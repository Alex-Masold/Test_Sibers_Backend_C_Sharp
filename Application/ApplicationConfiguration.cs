using Application.Contracts;
using Application.Contracts.EmployeeContracts;
using Application.Contracts.LoginContracts;
using Application.Contracts.ProjectContracts;
using Application.Contracts.TaskContracts;
using Application.Interfaces.Access;
using Application.Services;
using Application.Validators;
using Application.Validators.EmployeeValidators;
using Application.Validators.ProjectMemberValidators;
using Application.Validators.ProjectValidators;
using Application.Validators.TaskValidators;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddScoped<AuthService>();

        services.AddScoped<EmployeeService>();
        services.AddScoped<ProjectService>();
        services.AddScoped<ProjectMemberService>();
        services.AddScoped<ProjectDocumentService>();
        services.AddScoped<TaskService>();

        services.AddScoped<IProjectAccessValidator, ProjectAccessValidator>();
        services.AddScoped<IEmployeeAccessValidator, EmployeeAccessValidator>();
        services.AddScoped<IProjectMemberAccessValidator, ProjectMemberAccessValidator>();
        services.AddScoped<ITaskAccessValidator, TaskAccessValidator>();

        services.AddScoped<IValidator<ProjectCreateDto>, ProjectCreateDtoValidator>();
        services.AddScoped<IValidator<ProjectUpdateDto>, ProjectUpdateDtoValidator>();

        services.AddScoped<IValidator<EmployeeCreateDto>, EmployeeCreateDtoValidator>();
        services.AddScoped<IValidator<EmployeeUpdateDto>, EmployeeUpdateDtoValidator>();

        services.AddScoped<IValidator<TaskCreateDto>, TaskCreateDtoValidator>();
        services.AddScoped<IValidator<TaskUpdateDto>, TaskUpdateDtoValidator>();

        services.AddScoped<IValidator<LoginDto>, LoginDtoValidator>();
        services.AddScoped<IValidator<PagedDto>, PagedDtoValidator>();
        services.AddScoped<IValidator<IFormFile>, FileValidator>();
    }
}
