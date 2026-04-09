namespace Persistence.Base;

internal static class DbConstants
{
    // SQLite:
    public const string CaseInsensitiveCollation = "NOCASE";

    // SQL Server:
    // public const string CaseInsensitiveCollation = "SQL_Latin1_General_CP1_CI_AS";

    // PostgreSQL:
    // public const string CaseInsensitiveCollation = "und-x-icu";

    public const string Escape = "\\";
}
