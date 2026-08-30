using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Exceptions;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Common;
using SWA.Domain.Content.News;

namespace SWA.Application.Features.Public.News;

public sealed record GetNewsBySlugQuery(string Slug, string? Lang) : IRequest<NewsDetailDto>, ICacheableQuery
{
    public string CacheGroup => "News";
    public string CacheKey => $"slug:{Slug}:{Lang}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(5);
}

public sealed class GetNewsBySlugQueryHandler(IRepository<NewsArticle> repository, PublicContentOptions options) : IRequestHandler<GetNewsBySlugQuery, NewsDetailDto>
{
    public async Task<NewsDetailDto> Handle(GetNewsBySlugQuery request, CancellationToken cancellationToken)
    {
        if (!Slug.TryCreate(request.Slug, out var slug))
        {
            throw new NotFoundException(nameof(NewsArticle), request.Slug);
        }

        var article = await repository.Queryable()
            .Include(n => n.Translations)
            .PubliclyVisible(options)
            .FirstOrDefaultAsync(n => n.Slug == slug, cancellationToken)
            ?? throw new NotFoundException(nameof(NewsArticle), request.Slug);

        var translation = TranslationSelector.Pick(article.Translations, request.Lang)
            ?? throw new NotFoundException(nameof(NewsArticle), request.Slug);

        return new NewsDetailDto(
            article.Id,
            article.Slug.Value,
            translation.Title,
            translation.Summary,
            translation.Body,
            translation.HeroImageCaption,
            translation.SeoTitle,
            translation.SeoDescription,
            article.HeroImageId,
            article.IsFeatured,
            article.CreatedAtUtc,
            article.PublishedAtUtc);
    }
}
