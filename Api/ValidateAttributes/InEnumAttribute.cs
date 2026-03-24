using System.ComponentModel.DataAnnotations;

namespace Api.ValidateAttributes;

public class InEnumAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public InEnumAttribute(Type enumType)
    {
        if (!enumType.IsEnum)
            throw new ArgumentException($"Type {enumType.Name} is not an enum.");

        _enumType = enumType;
    }

    public override bool IsValid(object? value)
    {
        if (value == null)
            return true;

        return Enum.IsDefined(_enumType, value);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"Value is not valid for {name}. Allowed values are: {string.Join(", ", Enum.GetNames(_enumType))}";
    }
}
