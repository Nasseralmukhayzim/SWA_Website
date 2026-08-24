namespace SWA.Application.Features.Public.Events;

public sealed record EventListItemDto(Guid Id, string Slug, string Title, string? Location, DateTime StartsAtUtc, DateTime? EndsAtUtc, Guid? ImageId, string? EventTypeName);

public sealed record EventDetailDto(
    Guid Id,
    string Slug,
    string Title,
    string? Description,
    string? Location,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    Guid? ImageId,
    string? RegistrationUrl,
    string? EventTypeName);
