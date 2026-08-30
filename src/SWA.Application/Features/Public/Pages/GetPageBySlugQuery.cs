using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Exceptions;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Common;
using SWA.Domain.Content.Pages;

namespace SWA.Application.Features.Public.Pages;

public sealed record GetPageBySlugQuery(string Slug, string? Lang) : IRequest<PageDetailDto>, ICacheableQuery
{
    public string CacheGroup => "Pages";
    public string CacheKey => $"slug:{Slug}:{Lang}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(10);
}

public sealed class GetPageBySlugQueryHandler(IRepository<Page> repository, PublicContentOptions options) : IRequestHandler<GetPageBySlugQuery, PageDetailDto>
{
    public async Task<PageDetailDto> Handle(GetPageBySlugQuery request, CancellationToken cancellationToken)
    {
        if (!Slug.TryCreate(request.Slug, out var slug))
        {
            throw new NotFoundException(nameof(Page), request.Slug);
        }

        var page = await repository.Queryable()
            .Include(p => p.Translations)
            .PubliclyVisible(options)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken)
            ?? throw new NotFoundException(nameof(Page), request.Slug);

        var translation = TranslationSelector.Pick(page.Translations, request.Lang)
            ?? throw new NotFoundException(nameof(Page), request.Slug);

        var sections = (translation.Sections ?? [])
            .Select(section => new PageSectionDto(
                section.Kind.ToString(),
                section.Heading,
                section.Intro,
                section.Body,
                section.Items.Select(item => new PageSectionItemDto(item.Title, item.Description, item.IconId, item.ImageId, item.LinkUrl)).ToList()))
            .ToList();

        return new PageDetailDto(
            page.Id,
            page.Slug.Value,
            translation.Title,
            translation.Summary,
            translation.Body,
            translation.SeoTitle,
            translation.SeoDescription,
            page.HeroImageId,
            page.ParentId,
            page.ShowInNavigation,
            sections,
            page.UpdatedAtUtc);
    }
}
