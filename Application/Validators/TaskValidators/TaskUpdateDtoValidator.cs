using Application.Contracts.TaskContracts;
using Domain.Constants;
using Domain.Models;
using Domain.Stores;
using FluentValidation;

namespace Application.Validators.TaskValidators;

public class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
{
    private const int TitleMaxLength = FieldLimits.WorkTask.TitleMaxLength;

    private const int CommentMaxLength = FieldLimits.WorkTask.CommentMaxLength;

    public TaskUpdateDtoValidator(IProjectStore projectStore, IProjectMemberStore memberStore)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title cannot be empty")
            .MaximumLength(TitleMaxLength)
            .WithMessage($"Title must not exceed {TitleMaxLength} characters")
            .When(x => x.Title != null);

        RuleFor(x => x.Priority)
            .GreaterThan(0)
            .WithMessage("Priority must be greater than 0")
            .When(x => x.Priority.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid task status")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.Comment.Value)
            .MaximumLength(CommentMaxLength)
            .WithMessage($"Comment must not exceed {CommentMaxLength} characters")
            .When(x => x.Comment.HasValue);

        RuleFor(x => x.ProjectId)
            .MustAsync(
                async (projectId, ct) =>
                {
                    var project = await projectStore.GetByIdAsync(projectId!.Value, ct);
                    return project != null;
                }
            )
            .WithMessage("Project not found")
            .When(x => x.ProjectId.HasValue);

        RuleFor(x => x.ExecutorId.Value)
            .MustAsync(
                async (dto, executorId, ctx, ct) =>
                {
                    int? projectId = dto.ProjectId;

                    if (
                        !projectId.HasValue
                        && ctx.RootContextData.TryGetValue("ExistingTask", out var obj)
                        && obj is WorkTask existingTask
                    )
                    {
                        projectId = existingTask.ProjectId;
                    }

                    if (!projectId.HasValue)
                        return true;

                    return await memberStore.MemberExistAsync(
                        projectId.Value,
                        executorId.Value,
                        ct
                    );
                }
            )
            .WithMessage("Executor is not a member of this project")
            .When(x => x.ExecutorId.HasValue);

        RuleFor(dto => dto)
            .CustomAsync(
                async (dto, context, ct) =>
                {
                    if (
                        context.RootContextData.TryGetValue("ExistingTask", out var obj)
                        && obj is WorkTask existingTask
                    )
                    {
                        if (
                            dto.ProjectId.HasValue
                            && existingTask.ExecutorId.HasValue
                            && dto.ExecutorId.Value == null
                        )
                        {
                            var isMember = await memberStore.MemberExistAsync(
                                dto.ProjectId.Value,
                                existingTask.ExecutorId.Value,
                                ct
                            );
                            if (!isMember)
                                context.AddFailure(
                                    nameof(WorkTask.ProjectId),
                                    "Current executor is not a member of the new project"
                                );
                        }
                    }
                }
            );
    }
}
