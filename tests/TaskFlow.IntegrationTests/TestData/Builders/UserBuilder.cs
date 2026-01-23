using System;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.IntegrationTests.TestData.Builders;

public sealed class UserBuilder
{
    private string _userName = "test-user";
    private string _email = "test-user@test";

    public UserBuilder WithUserName(string userName)
    {
        _userName = userName;
        return this;
    }
    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithUniqueUserName(string prefix = "test-user")
    {
        _userName = $"{prefix}-{Guid.NewGuid():N}";
        return this;
    }

    public ApplicationUser Build()
    {
        return new()
        {
            UserName = _userName,
            Email = _email
        };
    }
        
}
