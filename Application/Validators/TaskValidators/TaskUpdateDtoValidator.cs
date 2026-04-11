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
        RuleFor(dto => dto.Title)
            .NotEmpty()
            .WithMessage("Title cannot be empty")
            .MaximumLength(TitleMaxLength)
            .WithMessage($"Title must not exceed {TitleMaxLength} characters")
            .When(dto => dto.Title != null);

        RuleFor(dto => dto.Priority)
            .GreaterThan(0)
            .WithMessage("Priority must be greater than 0")
            .When(dto => dto.Priority.HasValue);

        RuleFor(dto => dto.Status)
            .IsInEnum()
            .WithMessage("Invalid task status")
            .When(dto => dto.Status.HasValue);

        RuleFor(dto => dto.Comment.Value)
            .MaximumLength(CommentMaxLength)
            .WithMessage($"Comment must not exceed {CommentMaxLength} characters")
            .When(dto => dto.Comment.HasValue);

        RuleFor(dto => dto.ProjectId)
            .MustAsync(
                async (projectId, ct) =>
                {
                    var project = await projectStore.GetByIdAsync(projectId!.Value, ct);
                    return project != null;
                }
            )
            .WithMessage("Project not found")
            .When(dto => dto.ProjectId.HasValue);

        RuleFor(dto => dto.ExecutorId.Value)
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

                    if (!projectId.HasValue || !executorId.HasValue)
                        return true;

                    return await memberStore.MemberExistsAsync(
                        projectId.Value,
                        executorId.Value,
                        ct
                    );
                }
            )
            .WithMessage("Executor is not a member of this project")
            .When(dto => dto.ExecutorId.HasValue && dto.ExecutorId.Value.HasValue);

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
                            && !dto.ExecutorId.HasValue
                        )
                        {
                            var isMember = await memberStore.MemberExistsAsync(
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
