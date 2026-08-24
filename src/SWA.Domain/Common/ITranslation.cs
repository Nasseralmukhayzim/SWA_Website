namespace SWA.Domain.Common;

/// <summary>Implemented by every *Translation entity so public-API query handlers can pick a language generically.</summary>
public interface ITranslation
{
    string Language { get; }
}
