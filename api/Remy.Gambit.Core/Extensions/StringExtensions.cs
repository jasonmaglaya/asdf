namespace Remy.Gambit.Core.Extensions;

public static class StringExtensions
{
    public static string ThrowIfNullOrEmpty(this string text, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException($"{name ?? string.Empty} cannot be null or empty");
        }

        return text;
    }
}
