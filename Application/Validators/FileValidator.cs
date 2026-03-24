using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Validators;

public class FileValidator : AbstractValidator<IFormFile>
{
    private const long MaxFileSize = 50 * 1024 * 1024; // 50mb
    private static readonly HashSet<string> AllowedExtensions =
    [
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".png",
        ".jpg",
    ];

    public FileValidator()
    {
        RuleFor(file => file.Length)
            .GreaterThan(0)
            .WithMessage("File is empty")
            .LessThanOrEqualTo(MaxFileSize)
            .WithMessage($"File exceeds {MaxFileSize / 1024 / 1024}MB");

        RuleFor(file => file)
            .Custom(
                (file, context) =>
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!AllowedExtensions.Contains(ext))
                        context.AddFailure("Extension", $"Extension {ext} not allowed");
                }
            );
    }
}
