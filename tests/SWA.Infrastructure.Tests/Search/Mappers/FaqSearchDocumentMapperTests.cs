using SWA.Domain.Common;
using SWA.Domain.Content.Faqs;
using SWA.Infrastructure.Search.Mappers;

namespace SWA.Infrastructure.Tests.Search.Mappers;

public class FaqSearchDocumentMapperTests
{
    [Fact]
    public void Map_UsesQuestionAsTitle_AndAnswerAsBody()
    {
        var faq = new Faq { Id = Guid.NewGuid(), Slug = Slug.Create("how-to-pay-my-bill") };
        faq.Translations.Add(new FaqTranslation
        {
            Id = Guid.NewGuid(),
            FaqId = faq.Id,
            Question = "How do I pay my bill?",
            Answer = "You can pay online or at a branch.",
            Language = "en",
        });

        var doc = FaqSearchDocumentMapper.Map(faq).Single();

        Assert.Equal("How do I pay my bill?", doc.Title);
        Assert.Equal("You can pay online or at a branch.", doc.Body);
        Assert.Equal("Faq", doc.ContentType);
    }

    [Fact]
    public void Map_IncludesCategoryTaxonomy_WhenPresent()
    {
        var category = new FaqCategory { Id = Guid.NewGuid(), Slug = Slug.Create("billing") };
        category.Translations.Add(new FaqCategoryTranslation { Id = Guid.NewGuid(), FaqCategoryId = category.Id, Name = "Billing", Language = "en" });

        var faq = new Faq { Id = Guid.NewGuid(), Slug = Slug.Create("how-to-pay-my-bill"), Category = category };
        faq.Translations.Add(new FaqTranslation { Id = Guid.NewGuid(), FaqId = faq.Id, Question = "Q", Answer = "A", Language = "en" });

        var doc = FaqSearchDocumentMapper.Map(faq).Single();

        Assert.Equal(["Billing"], doc.TaxonomyLabels);
        Assert.Equal(["billing"], doc.TaxonomySlugs);
    }

    [Fact]
    public void Map_WithoutCategory_ProducesEmptyTaxonomy()
    {
        var faq = new Faq { Id = Guid.NewGuid(), Slug = Slug.Create("q") };
        faq.Translations.Add(new FaqTranslation { Id = Guid.NewGuid(), FaqId = faq.Id, Question = "Q", Answer = "A", Language = "en" });

        var doc = FaqSearchDocumentMapper.Map(faq).Single();

        Assert.Empty(doc.TaxonomyLabels);
        Assert.Empty(doc.TaxonomySlugs);
    }
}
