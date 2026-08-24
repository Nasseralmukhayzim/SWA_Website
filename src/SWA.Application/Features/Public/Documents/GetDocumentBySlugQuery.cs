using MediatR;
using Microsoft.EntityFrameworkCore;
using SWA.Application.Common.Content;
using SWA.Application.Common.Exceptions;
using SWA.Application.Common.Interfaces;
using SWA.Application.Common.Localization;
using SWA.Domain.Common;
using SWA.Domain.Content.Documents;

namespace SWA.Application.Features.Public.Documents;

public sealed record GetDocumentBySlugQuery(string Slug, string? Lang) : IRequest<DocumentDetailDto>;

public sealed class GetDocumentBySlugQueryHandler(IRepository<Document> repository, PublicContentOptions options) : IRequestHandler<GetDocumentBySlugQuery, DocumentDetailDto>
{
    public async Task<DocumentDetailDto> Handle(GetDocumentBySlugQuery request, CancellationToken cancellationToken)
    {
        if (!Slug.TryCreate(request.Slug, out var slug))
        {
            throw new NotFoundException(nameof(Document), request.Slug);
        }

        var document = await repository.Queryable()
            .Include(d => d.Translations)
            .Include(d => d.Category!.Translations)
            .PubliclyVisible(options)
            .FirstOrDefaultAsync(d => d.Slug == slug, cancellationToken)
            ?? throw new NotFoundException(nameof(Document), request.Slug);

        var translation = TranslationSelector.Pick(document.Translations, request.Lang)
            ?? throw new NotFoundException(nameof(Document), request.Slug);
        var categoryTranslation = document.Category is null ? null : TranslationSelector.Pick(document.Category.Translations, request.Lang);

        return new DocumentDetailDto(
            document.Id,
            document.Slug.Value,
            translation.Title,
            translation.Description,
            (int)document.Section,
            document.Year,
            document.CoverImageId,
            translation.FileId,
            translation.ExternalFileUrl,
            categoryTranslation?.Name);
    }
}
