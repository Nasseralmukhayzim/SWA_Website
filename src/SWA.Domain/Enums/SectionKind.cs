namespace SWA.Domain.Enums;

/// <summary>Serialized as a string (not int) inside PageTranslation.Sections JSON to match the CMS's SectionKind union type.</summary>
public enum SectionKind
{
    Text,
    CardGrid,
    StatGroup,
    Timeline,
}
