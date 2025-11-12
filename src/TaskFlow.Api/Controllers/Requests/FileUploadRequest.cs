using Microsoft.AspNetCore.Http;

namespace TaskFlow.Api.Controllers.Requests;

public class FileUploadRequest
{
    public string Container { get; set; } = "files";
    public IFormFile File { get; set; } = default!;
}
