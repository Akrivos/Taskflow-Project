using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.Infrastructure.Persistence;
using Xunit;

namespace TaskFlow.IntegrationTests.Projects;

public class CreateProjectEndpointTests : IClassFixture<TaskFlowApiFactory>
{
    private readonly TaskFlowApiFactory _factory;

    public CreateProjectEndpointTests(TaskFlowApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_Projects_Should_Create_Project_And_Return_201()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test");

        client.DefaultRequestHeaders.Add("X-Test-Role", "Admin");

        var request = new
        {
            Name = "Integration Test Project",
            Description = "Created from integration test"
        };

        var response = await client.PostAsJsonAsync("/api/projects", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();

        var project = await db.Projects.SingleOrDefaultAsync(p => p.Name == request.Name);
        project.Should().NotBeNull();
        project!.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task Post_Projects_With_User_Role_Should_Return_403()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Test");

        client.DefaultRequestHeaders.Add("X-Test-User", "test-user-id");
        client.DefaultRequestHeaders.Add("X-Test-Role", "User");

        var body = new
        {
            Name = "Test Name 1",
            Description = "Test description 1"
        };

        var response = await client.PostAsJsonAsync("/api/projects", body);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
