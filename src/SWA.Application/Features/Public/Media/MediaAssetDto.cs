namespace SWA.Application.Features.Public.Media;

public sealed record MediaAssetDto(Guid Id, string Url, string ContentType, int Kind, string? TitleAr, string? TitleEn, string? AltTextAr, string? AltTextEn);
