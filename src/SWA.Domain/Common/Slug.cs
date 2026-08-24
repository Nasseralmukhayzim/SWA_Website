using System.Text.RegularExpressions;

namespace SWA.Domain.Common;

/// <summary>Lowercase-hyphen identifier shared by every content type and lookup (mirrors the CMS's SLUG_PATTERN/SLUG_MAX_LENGTH).</summary>
public readonly partial struct Slug : IEquatable<Slug>
{
    public const int MaxLength = 200;

    public string Value { get; }

    private Slug(string value) => Value = value;

    public static Slug Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Slug is required.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Slug must be at most {MaxLength} characters.", nameof(value));
        if (!SlugPattern().IsMatch(value))
            throw new ArgumentException("Slug must contain only lowercase letters, numbers, and hyphens.", nameof(value));

        return new Slug(value);
    }

    public static bool TryCreate(string? value, out Slug slug)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Length <= MaxLength && SlugPattern().IsMatch(value))
        {
            slug = new Slug(value);
            return true;
        }

        slug = default;
        return false;
    }

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex SlugPattern();

    public bool Equals(Slug other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is Slug other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(Slug left, Slug right) => left.Equals(right);
    public static bool operator !=(Slug left, Slug right) => !left.Equals(right);

    public static implicit operator string(Slug slug) => slug.Value;
    public static explicit operator Slug(string value) => Create(value);
}
