namespace TaskFlow.Api.Controllers.Requests;

public class FileUploadRequest
{
    public string Container { get; set; } = default!;
    public IFormFile File { get; set; } = default!;
}
