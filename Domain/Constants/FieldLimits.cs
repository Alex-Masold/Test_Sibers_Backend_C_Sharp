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
}
