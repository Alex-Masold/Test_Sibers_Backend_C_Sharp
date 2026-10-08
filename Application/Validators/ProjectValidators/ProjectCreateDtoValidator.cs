using Application.Contracts.ProjectContracts;
using Application.Validators.Rules;
using Domain.Constants;
using Domain.Models;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.ProjectValidators;

public class ProjectCreateDtoValidator : AbstractValidator<ProjectCreateDto>
{
    private const int NameMaxLength = FieldLimits.Project.NameMaxLength;

    private const int CompanyNameMaxLength = FieldLimits.Project.CompanyNameMaxLength;

    public ProjectCreateDtoValidator(IEmployeeStore employeeStore)
    {
        RuleFor(dto => dto.Name).ApplyProjectNameRules();

        RuleFor(x => x.Priority)
            .InclusiveBetween(1, 5)
            .WithMessage($"The priority must be between 1 and 5");

        RuleFor(dto => dto.CompanyOrdering).ApplyCompanyNameRules();

        RuleFor(dto => dto.CompanyExecuting)
            .ApplyOptionalCompanyNameRules()
            .When(dto => !string.IsNullOrEmpty(dto.CompanyExecuting));

        RuleFor(dto => dto.EndDate)
            .GreaterThanOrEqualTo(dto => dto.StartDate)
            .WithMessage("The deadline cannot be earlier than the start date")
            .When(dto => dto.EndDate.HasValue);

        RuleFor(dto => dto.ManagerId)
            .MustAsync(
                async (managerId, ct) =>
                {
                    var employee = await employeeStore.GetByIdAsync(managerId!.Value, ct);
                    return employee is not null && employee.Role == Role.Manager;
                }
            )
            .WithMessage("Manager must exist and have Manager role")
            .When(dto => dto.ManagerId.HasValue);
    }
}
