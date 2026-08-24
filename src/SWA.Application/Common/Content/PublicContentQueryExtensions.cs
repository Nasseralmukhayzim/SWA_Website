using SWA.Domain.Content;
using SWA.Domain.Enums;

namespace SWA.Application.Common.Content;

public static class PublicContentQueryExtensions
{
    /// <summary>
    /// Restricts a content query to the rows the public site may show. This is the single place the
    /// public visibility rule lives — the six content types must not re-state it themselves.
    /// </summary>
    public static IQueryable<T> PubliclyVisible<T>(this IQueryable<T> source, PublicContentOptions options)
        where T : PublishableContent
    {
        source = source.Where(e => !e.IsDeleted && e.DeletionStatus != DeletionRequestStatus.Approved);

        return options.IncludeUnpublished
            ? source
            : source.Where(e => e.Status == ContentStatus.Published);
    }
}
