namespace SWA.Application.Common.Search;

/// <summary>Elasticsearch connection and sync-cadence settings for the public search index.</summary>
public sealed class ElasticsearchOptions
{
    public const string SectionName = "Elasticsearch";

    public string Uri { get; set; } = "http://localhost:9200";
    public string IndexName { get; set; } = "swa-content";
    public int SyncIntervalSeconds { get; set; } = 90;
    public int FullSweepEveryNTicks { get; set; } = 10;

    /// <summary>MIME types worth opening and running through the attachment ingest pipeline (PDF/Word); anything else is skipped.</summary>
    public string[] AttachmentContentTypes { get; set; } =
        ["application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"];

    /// <summary>Files larger than this are skipped rather than read and base64-encoded on every sync tick.</summary>
    public long MaxAttachmentSizeBytes { get; set; } = 10_000_000;

    /// <summary>Caps how much extracted text the attachment ingest processor keeps per file.</summary>
    public int AttachmentIndexedChars { get; set; } = 100_000;
}
