using Newtonsoft.Json.Serialization;

namespace FC.Codeflix.Catalog.EndToEndTests.Extensions;

public static class StringSnakeCaseExtension
{
    private static readonly NamingStrategy SnakeCaseNamingStrategy = new SnakeCaseNamingStrategy();

    public static string ToSnakeCase(this string str)
    {
        ArgumentNullException.ThrowIfNull(str, nameof(str));

        return SnakeCaseNamingStrategy.GetPropertyName(str, false);
    }
}