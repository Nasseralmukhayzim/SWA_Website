using SWA.Domain.Common;
using SWA.Domain.Content.Documents;
using SWA.Infrastructure.Search.Mappers;

namespace SWA.Infrastructure.Tests.Search.Mappers;

public class DocumentSearchDocumentMapperTests
{
    [Fact]
    public void Map_WithNullDescription_ProducesEmptyBody()
    {
        var document = new Document { Id = Guid.NewGuid(), Slug = Slug.Create("annual-report-2026") };
        document.Translations.Add(new DocumentTranslation { Id = Guid.NewGuid(), DocumentId = document.Id, Title = "Annual Report 2026", Language = "en" });

        var doc = DocumentSearchDocumentMapper.Map(document).Single();

        Assert.Equal("Annual Report 2026", doc.Title);
        Assert.Equal(string.Empty, doc.Body);
        Assert.Equal("Document", doc.ContentType);
    }

    [Fact]
    public void Map_IncludesCategoryTaxonomy_WhenPresent()
    {
        var category = new DocumentCategory { Id = Guid.NewGuid(), Slug = Slug.Create("reports") };
        category.Translations.Add(new DocumentCategoryTranslation { Id = Guid.NewGuid(), DocumentCategoryId = category.Id, Name = "Reports", Language = "en" });

        var document = new Document { Id = Guid.NewGuid(), Slug = Slug.Create("annual-report-2026"), Category = category };
        document.Translations.Add(new DocumentTranslation { Id = Guid.NewGuid(), DocumentId = document.Id, Title = "Annual Report 2026", Language = "en" });

        var doc = DocumentSearchDocumentMapper.Map(document).Single();

        Assert.Equal(["Reports"], doc.TaxonomyLabels);
        Assert.Equal(["reports"], doc.TaxonomySlugs);
    }

    [Fact]
    public void Map_WithMatchingFileId_PassesAttachmentBase64Through()
    {
        var fileId = Guid.NewGuid();
        var document = new Document { Id = Guid.NewGuid(), Slug = Slug.Create("license-terms") };
        document.Translations.Add(new DocumentTranslation { Id = Guid.NewGuid(), DocumentId = document.Id, Title = "License Terms", FileId = fileId, Language = "en" });

        var attachments = new Dictionary<Guid, string> { [fileId] = "QkFTRTY0" };

        var doc = DocumentSearchDocumentMapper.Map(document, attachmentsByFileId: attachments).Single();

        Assert.Equal("QkFTRTY0", doc.AttachmentBase64);
    }

    [Fact]
    public void Map_WithFileIdNotInAttachmentsDictionary_LeavesAttachmentBase64Null()
    {
        var document = new Document { Id = Guid.NewGuid(), Slug = Slug.Create("license-terms") };
        document.Translations.Add(new DocumentTranslation { Id = Guid.NewGuid(), DocumentId = document.Id, Title = "License Terms", FileId = Guid.NewGuid(), Language = "en" });

        var doc = DocumentSearchDocumentMapper.Map(document, attachmentsByFileId: new Dictionary<Guid, string>()).Single();

        Assert.Null(doc.AttachmentBase64);
    }
}
