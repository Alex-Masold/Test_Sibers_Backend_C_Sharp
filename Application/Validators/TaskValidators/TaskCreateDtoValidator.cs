using Application.Contracts.TaskContracts;
using Domain.Constants;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.TaskValidators;

public class TaskCreateDtoValidator : AbstractValidator<TaskCreateDto>
{
    private const int TitleMaxLength = FieldLimits.WorkTask.TitleMaxLength;

    private const int CommentMaxLength = FieldLimits.WorkTask.CommentMaxLength;

    public TaskCreateDtoValidator(IProjectMemberStore memberStore)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(TitleMaxLength)
            .WithMessage($"Title must not exceed {TitleMaxLength} characters");

        RuleFor(x => x.Priority).GreaterThan(0).WithMessage("Priority must be greater than 0");

        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid task status");

        RuleFor(x => x.Comment)
            .MaximumLength(CommentMaxLength)
            .WithMessage($"Comment must not exceed {CommentMaxLength} characters")
            .When(x => x.Comment is not null);

        RuleFor(x => x.ExecutorId)
            .MustAsync(
                async (dto, executorId, ctx, ct) =>
                {
                    return await memberStore.MemberExistsAsync(
                        dto.ProjectId,
                        executorId!.Value,
                        ct
                    );
                }
            )
            .WithMessage("Executor is not a member of this project")
            .When(x => x.ExecutorId.HasValue);
    }
}
