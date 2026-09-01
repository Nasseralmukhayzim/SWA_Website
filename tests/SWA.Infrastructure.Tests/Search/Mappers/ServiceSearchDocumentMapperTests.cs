using SWA.Domain.Common;
using SWA.Domain.Content.Services;
using SWA.Infrastructure.Search.Mappers;

namespace SWA.Infrastructure.Tests.Search.Mappers;

public class ServiceSearchDocumentMapperTests
{
    private static Service CreateService(params ServiceTranslation[] translations)
    {
        var service = new Service
        {
            Id = Guid.NewGuid(),
            Slug = Slug.Create("water-connection"),
            SortOrder = 3,
            UpdatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PublishedAtUtc = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc),
        };
        service.Translations.AddRange(translations);
        return service;
    }

    [Fact]
    public void Map_ProducesOneDocumentPerTranslation()
    {
        var service = CreateService(
            new ServiceTranslation { Id = Guid.NewGuid(), ServiceId = Guid.Empty, Name = "Water Connection", Language = "en" },
            new ServiceTranslation { Id = Guid.NewGuid(), ServiceId = Guid.Empty, Name = "توصيل المياه", Language = "ar" });

        var docs = ServiceSearchDocumentMapper.Map(service).ToList();

        Assert.Equal(2, docs.Count);
        Assert.Contains(docs, d => d.Language == "en" && d.Title == "Water Connection");
        Assert.Contains(docs, d => d.Language == "ar" && d.Title == "توصيل المياه");
    }

    [Fact]
    public void Map_UsesNameAsTitle_AndConcatenatesProseFieldsIntoBody_SkippingNulls()
    {
        var service = CreateService(new ServiceTranslation
        {
            Id = Guid.NewGuid(),
            ServiceId = Guid.Empty,
            Name = "Water Connection",
            Language = "en",
            Description = "Connects your property to the network.",
            Fee = null,
            DeliveryTime = "5 business days",
            RequiredDocuments = null,
            Steps = "Apply, pay, wait",
            Terms = null,
            Objectives = null,
        });

        var doc = ServiceSearchDocumentMapper.Map(service).Single();

        Assert.Equal("Water Connection", doc.Title);
        Assert.Equal("Connects your property to the network. | 5 business days | Apply, pay, wait", doc.Body);
    }

    [Fact]
    public void Map_IncludesCategoryAndActivityTypeAsTaxonomy()
    {
        var category = new ServiceCategory { Id = Guid.NewGuid(), Slug = Slug.Create("networked-water") };
        category.Translations.Add(new ServiceCategoryTranslation { Id = Guid.NewGuid(), ServiceCategoryId = category.Id, Name = "Networked Water", Language = "en" });

        var activityType = new ServiceActivityType { Id = Guid.NewGuid(), Slug = Slug.Create("distribution") };
        activityType.Translations.Add(new ServiceActivityTypeTranslation { Id = Guid.NewGuid(), ServiceActivityTypeId = activityType.Id, Name = "Distribution", Language = "en" });

        var service = CreateService(new ServiceTranslation { Id = Guid.NewGuid(), ServiceId = Guid.Empty, Name = "Water Connection", Language = "en" });
        service.ServiceCategory = category;
        service.ServiceActivityType = activityType;

        var doc = ServiceSearchDocumentMapper.Map(service).Single();

        Assert.Equal(["Networked Water", "Distribution"], doc.TaxonomyLabels);
        Assert.Equal(["networked-water", "distribution"], doc.TaxonomySlugs);
    }

    [Fact]
    public void Map_WithLanguageFilter_SkipsEntityWithNoMatchingTranslation()
    {
        var service = CreateService(new ServiceTranslation { Id = Guid.NewGuid(), ServiceId = Guid.Empty, Name = "Water Connection", Language = "en" });

        var docs = ServiceSearchDocumentMapper.Map(service, "ar").ToList();

        Assert.Empty(docs);
    }

    [Fact]
    public void Map_SetsCompositeIdAndCarriesEntityMetadata()
    {
        var service = CreateService(new ServiceTranslation { Id = Guid.NewGuid(), ServiceId = Guid.Empty, Name = "Water Connection", Language = "en" });

        var doc = ServiceSearchDocumentMapper.Map(service).Single();

        Assert.Equal($"Service:{service.Id}:en", doc.Id);
        Assert.Equal(service.Id, doc.EntityId);
        Assert.Equal("Service", doc.ContentType);
        Assert.Equal("water-connection", doc.Slug);
        Assert.Equal(service.UpdatedAtUtc, doc.UpdatedAtUtc);
        Assert.Equal(service.PublishedAtUtc, doc.PublishedAtUtc);
        Assert.Equal(service.SortOrder, doc.SortOrder);
    }
}
