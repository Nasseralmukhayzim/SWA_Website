namespace SWA.Application.Features.Public.Services;

public sealed record ServiceListItemDto(
    Guid Id,
    string Slug,
    string Name,
    string? Description,
    int DeliveryType,
    Guid? IconId,
    bool IsFeatured,
    IReadOnlyList<string> AudienceNames,
    // Slugs as well as names: the services page filters by audience client-side, and matching
    // on a localised display name would break as soon as an editor rewords one.
    IReadOnlyList<string> AudienceSlugs,
    // The service family, for the category chips on the e-services page. Null when an editor
    // has not filed the service under one yet.
    string? CategorySlug,
    string? CategoryName,
    // Where the service sits in the water value chain, for the "نوع النشاط" tabs.
    string? ActivityTypeSlug,
    string? ActivityTypeName);

public sealed record ServiceDetailDto(
    Guid Id,
    string Slug,
    string Name,
    string? Description,
    int DeliveryType,
    Guid? IconId,
    string? SupportPhone,
    bool IsFeatured,
    string? Fee,
    string? DeliveryTime,
    string? RequiredDocuments,
    string? Steps,
    string? Terms,
    string? Objectives,
    string? StartServiceUrl,
    Guid? GuideFileId,
    IReadOnlyList<string> AudienceSlugs,
    IReadOnlyList<string> ChannelSlugs,
    // Display names alongside the slugs: the detail page shows these to beneficiaries, and a slug
    // ("business") is not something to put in front of a reader.
    IReadOnlyList<string> AudienceNames,
    IReadOnlyList<string> ChannelNames,
    // The same two lookups the list carries: the detail page shows all three as tags under the
    // title, so it needs the names, and it finds related services by matching the category slug.
    string? CategorySlug,
    string? CategoryName,
    string? ActivityTypeSlug,
    string? ActivityTypeName,
    DateTime? UpdatedAtUtc);
