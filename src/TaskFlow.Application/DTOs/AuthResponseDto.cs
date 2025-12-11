using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs;

public record AuthResponseDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public IList<string> Roles { get; set; }
}