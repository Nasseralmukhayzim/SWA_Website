namespace SWA.Application.Features.Public.Faqs;

public sealed record FaqListItemDto(Guid Id, string Slug, string Question, string Answer, Guid? CategoryId, string? CategoryName);
