using Application.Contracts.ProjectContracts;
using Domain.Constants;
using Domain.Models;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.ProjectValidators;

public class ProjectUpdateDtoValidator : AbstractValidator<ProjectUpdateDto>
{
    private const int NameMaxLength = FieldLimits.Project.NameMaxLength;

    private const int CompanyNameMaxLength = FieldLimits.Project.CompanyNameMaxLength;

    public ProjectUpdateDtoValidator(IEmployeeStore employeeStore)
    {
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .MaximumLength(NameMaxLength)
            .WithMessage($"Name must not exceed {NameMaxLength} characters")
            .When(dto => dto.Name is not null);

        RuleFor(dto => dto.Priority)
            .InclusiveBetween(1, 5)
            .WithMessage($"The priority must be between 1 and 5")
            .When(dto => dto.Priority.HasValue);

        RuleFor(dto => dto.CompanyOrdering)
            .NotEmpty()
            .WithMessage("Company ordering cannot be empty")
            .MaximumLength(CompanyNameMaxLength)
            .WithMessage($"Company Name must not exceed {CompanyNameMaxLength} characters")
            .When(dto => dto.CompanyOrdering is not null);

        RuleFor(dto => dto.CompanyExecuting.Value)
            .MaximumLength(CompanyNameMaxLength)
            .WithMessage($"Company Name must not exceed {CompanyNameMaxLength} characters")
            .When(dto => dto.CompanyExecuting.HasValue && !string.IsNullOrEmpty(dto.CompanyExecuting.Value));

        RuleFor(dto => dto.StartDate)
            .GreaterThan(new DateOnly(2000, 1, 1))
            .WithMessage("StartDate date must be after 2000-01-01")
            .When(dto => dto.StartDate.HasValue);

        RuleFor(dto => dto)
            .Custom(
                (dto, context) =>
                {
                    if (
                        context.RootContextData.TryGetValue("ExistingProject", out var obj)
                        && obj is Project existingProject
                    )
                    {
                        var newStart = dto.StartDate ?? existingProject.StartDate;
                        var newEnd = dto.EndDate.HasValue
                            ? dto.EndDate.Value
                            : existingProject.EndDate;

                        if (newEnd.HasValue && newEnd < newStart)
                        {
                            context.AddFailure(
                                nameof(Project.EndDate),
                                "The deadline cannot be earlier than the start date"
                            );
                        }
                    }
                }
            );

        RuleFor(dto => dto.ManagerId.Value)
            .MustAsync(
                async (managerId, ct) =>
                {
                    var employee = await employeeStore.GetByIdAsync(managerId!.Value, ct);

                    return employee != null && employee.Role == Role.Manager;
                }
            )
            .WithMessage("Manager must exist and have Manager role")
            .When(dto => dto.ManagerId.HasValue && dto.ManagerId.Value.HasValue);
    }
}
