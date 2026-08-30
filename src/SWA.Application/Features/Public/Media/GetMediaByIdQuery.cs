using MediatR;
using SWA.Application.Common.Exceptions;
using SWA.Application.Common.Interfaces;
using SWA.Domain.Media;

namespace SWA.Application.Features.Public.Media;

public sealed record GetMediaByIdQuery(Guid Id) : IRequest<MediaAssetDto>, ICacheableQuery
{
    public string CacheGroup => "Media";
    public string CacheKey => $"{Id}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(30);
}

public sealed class GetMediaByIdQueryHandler(IRepository<MediaAsset> repository) : IRequestHandler<GetMediaByIdQuery, MediaAssetDto>
{
    public async Task<MediaAssetDto> Handle(GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        var asset = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(MediaAsset), request.Id);

        if (asset.IsDeleted)
        {
            throw new NotFoundException(nameof(MediaAsset), request.Id);
        }

        return new MediaAssetDto(asset.Id, $"/uploads/{asset.StorageKey}", asset.ContentType, (int)asset.Kind, asset.TitleAr, asset.TitleEn, asset.AltTextAr, asset.AltTextEn);
    }
}
