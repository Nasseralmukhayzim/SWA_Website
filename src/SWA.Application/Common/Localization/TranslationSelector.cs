using SWA.Domain.Common;

namespace SWA.Application.Common.Localization;

public static class TranslationSelector
{
    public const string DefaultLanguage = "en";

    /// <summary>Picks the requested language's translation, falling back to the other available language rather than 404ing.</summary>
    public static T? Pick<T>(IEnumerable<T> translations, string? language) where T : ITranslation
    {
        var requested = string.IsNullOrWhiteSpace(language) ? DefaultLanguage : language.Trim().ToLowerInvariant();
        var list = translations as IReadOnlyCollection<T> ?? translations.ToList();

        return list.FirstOrDefault(t => t.Language.Equals(requested, StringComparison.OrdinalIgnoreCase))
            ?? list.FirstOrDefault(t => t.Language.Equals(DefaultLanguage, StringComparison.OrdinalIgnoreCase))
            ?? list.FirstOrDefault();
    }
}
