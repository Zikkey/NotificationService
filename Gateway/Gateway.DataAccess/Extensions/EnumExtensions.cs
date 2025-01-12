namespace Gateway.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetDbFriendlyValue(this Enum value) => value.ToString().ToSnakeCase();
}
