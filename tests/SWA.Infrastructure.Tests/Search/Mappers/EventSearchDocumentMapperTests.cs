using SWA.Domain.Common;
using SWA.Domain.Content.Events;
using SWA.Infrastructure.Search.Mappers;

namespace SWA.Infrastructure.Tests.Search.Mappers;

public class EventSearchDocumentMapperTests
{
    [Fact]
    public void Map_UsesDescriptionAndLocationAsBody_NoBodyField()
    {
        var @event = new Event { Id = Guid.NewGuid(), Slug = Slug.Create("water-summit") };
        @event.Translations.Add(new EventTranslation
        {
            Id = Guid.NewGuid(),
            EventId = @event.Id,
            Title = "Water Summit",
            Description = "Annual summit.",
            Location = "Main Hall",
            Language = "en",
        });

        var doc = EventSearchDocumentMapper.Map(@event).Single();

        Assert.Equal("Water Summit", doc.Title);
        Assert.Equal("Annual summit. | Main Hall", doc.Body);
        Assert.Equal("Event", doc.ContentType);
    }
}
