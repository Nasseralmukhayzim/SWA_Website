namespace SWA.Infrastructure.Persistence;

/// <summary>
/// Mirrors the table the CMS owns and writes to (see its CacheGroupVersion). This app only ever
/// reads it — see the ExcludeFromMigrations call in ApplicationDbContext.OnModelCreating.
/// </summary>
public sealed class CacheGroupVersion
{
    public required string GroupName { get; init; }
    public DateTime UpdatedAtUtc { get; set; }
}
