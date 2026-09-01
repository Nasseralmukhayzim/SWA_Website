using SWA.Domain.Content.Pages;
using SWA.Domain.Enums;
using SWA.Infrastructure.Search;

namespace SWA.Infrastructure.Tests.Search;

public class SearchVisibilityTests
{
    private static Page CreatePage(ContentStatus status, bool isDeleted, DeletionRequestStatus deletionStatus) => new()
    {
        Id = Guid.NewGuid(),
        Status = status,
        IsDeleted = isDeleted,
        DeletionStatus = deletionStatus,
    };

    [Fact]
    public void IsPubliclyVisible_PublishedNotDeleted_ReturnsTrue()
    {
        var page = CreatePage(ContentStatus.Published, isDeleted: false, DeletionRequestStatus.None);

        Assert.True(SearchVisibility.IsPubliclyVisible(page));
    }

    [Theory]
    [InlineData(ContentStatus.Draft)]
    [InlineData(ContentStatus.InReview)]
    [InlineData(ContentStatus.Approved)]
    [InlineData(ContentStatus.Archived)]
    public void IsPubliclyVisible_NotPublished_ReturnsFalse(ContentStatus status)
    {
        var page = CreatePage(status, isDeleted: false, DeletionRequestStatus.None);

        Assert.False(SearchVisibility.IsPubliclyVisible(page));
    }

    [Fact]
    public void IsPubliclyVisible_SoftDeleted_ReturnsFalse()
    {
        var page = CreatePage(ContentStatus.Published, isDeleted: true, DeletionRequestStatus.None);

        Assert.False(SearchVisibility.IsPubliclyVisible(page));
    }

    [Fact]
    public void IsPubliclyVisible_DeletionApproved_ReturnsFalse()
    {
        var page = CreatePage(ContentStatus.Published, isDeleted: false, DeletionRequestStatus.Approved);

        Assert.False(SearchVisibility.IsPubliclyVisible(page));
    }

    [Fact]
    public void IsPubliclyVisible_DeletionRequestedButNotApproved_StillVisible()
    {
        var page = CreatePage(ContentStatus.Published, isDeleted: false, DeletionRequestStatus.Requested);

        Assert.True(SearchVisibility.IsPubliclyVisible(page));
    }
}
