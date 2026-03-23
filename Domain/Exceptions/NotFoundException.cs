namespace Domain.Exceptions;

public class NotFoundException : Exception
{
    public string EntityName { get; }
    public object? Key { get; }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.")
    {
        EntityName = entityName;
        Key = key;
    }

    public NotFoundException(string entityName, IReadOnlyCollection<object> keyList)
        : base($"{entityName} with id [{string.Join(", ", keyList)}] not exist")
    {
        EntityName = entityName;
        Key = keyList;
    }
}
