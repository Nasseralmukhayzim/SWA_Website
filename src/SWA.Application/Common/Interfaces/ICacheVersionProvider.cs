namespace SWA.Application.Common.Interfaces;

public interface ICacheVersionProvider
{
    Task<long> GetVersionAsync(string group, CancellationToken cancellationToken);
}
