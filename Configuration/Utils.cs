using FluentValidation;

namespace Teraa.Extensions.Configuration;

internal static class Utils
{
    public static string GetSectionPathFromType(Type optionsType)
    {
        const string suffix = "Options";

        string path = optionsType.Name;

        if (path.EndsWith(suffix))
        {
            path = path[..^suffix.Length];
        }

        return path;
    }
}
