using TaskFlow.Domain.Common;
namespace TaskFlow.Domain.Entities;
public class Attachment : AuditableEntity
{
    public string FileName { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public string BlobUrl { get; private set; } = default!;
    public Guid TaskItemId { get; private set; }
    public TaskItem TaskItem { get; private set; } = default!;
    public Attachment() { }
    public Attachment(string fileName, string contentType, string blobUrl, Guid taskItemId)
    {
        FileName = fileName;
        ContentType = contentType;
        BlobUrl = blobUrl;
        TaskItemId = taskItemId;
    }
}
