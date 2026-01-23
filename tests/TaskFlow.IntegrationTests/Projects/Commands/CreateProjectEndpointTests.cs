using FluentAssertions;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.Api.Controllers.Requests.Projects;
using TaskFlow.Domain.Entities;
using TaskFlow.IntegrationTests.Infrastructure;
using Xunit;

namespace TaskFlow.IntegrationTests.Projects.Commands;

[Collection("IntegrationTests")]
public class CreateProjectEndpointTests : IntegrationTestBase
{
    public CreateProjectEndpointTests(TaskFlowApiFactory factory) : base(factory) { }


    [Fact]
    public async Task Create_Project_AsAdmin_ReturnsCreated_AndPersistsToDb()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "Admin", id: "test-user-id", username: "test-user");

        CreateProjectRequest request = new CreateProjectRequest(
            Name: "Integration Test Project",
            Description: "Created from integration test"
        );

        var response = await client.PostAsJsonAsync("/api/projects", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var project = await TestDb.SingleAsync<Project>(
            db => db.Projects.Where(p => p.Name == request.Name)
         );

        project.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task Create_Project_AsUser_ReturnsForbidden()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "User", id: "test-user-id", username: "test-user");

        var response = await client.PostAsJsonAsync("/api/projects", new CreateProjectRequest(
            Name: "Test Name 1",
            Description: "Test description 1"
        ));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
