namespace Domain.Constants;

public static class FieldLimits
{
    public static class Employee
    {
        public const int FirstNameMaxLength = 200;
        public const int MiddleNameMaxLength = 200;
        public const int LastNameMaxLength = 200;
        public const int EmailMaxLength = 200;
    }

    public static class Project
    {
        public const int NameMaxLength = 100;
        public const int CompanyNameMaxLength = 150;
    }

    public static class WorkTask
    {
        public const int TitleMaxLength = 200;
        public const int CommentMaxLength = 1000;
    }

    public static class Password
    {
        public const int MinLength = 8;
        public const int MaxLength = 128;
        public const string UppercasePattern = @"[A-Z]";
        public const string LowercasePattern = @"[a-z]";
        public const string DigitPattern = @"[0-9]";
        public const string SpecialCharacterPattern = @"[\W_]";
    }
}
