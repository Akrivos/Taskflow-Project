using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string userName, IEnumerable<string> roles);
}