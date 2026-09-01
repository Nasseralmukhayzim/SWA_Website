namespace SWA.Application.Common.Interfaces;

/// <summary>Search-engine abstraction — implemented against Elasticsearch in SWA.Infrastructure, kept out of Application so the query/handler layer never depends on the ES client.</summary>
public interface ISearchIndexer
{
    Task EnsureIndexAsync(CancellationToken cancellationToken);
    Task UpsertAsync(IReadOnlyCollection<SearchDocument> documents, CancellationToken cancellationToken);
    Task DeleteByEntityAsync(string contentType, Guid entityId, CancellationToken cancellationToken);
    Task<SearchResultPage> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
}
