using FluentAssertions;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.Api.Controllers.Requests.Projects;
using TaskFlow.Domain.Entities;
using TaskFlow.IntegrationTests.Infrastructure;
using TaskFlow.IntegrationTests.TestData.Builders;
using TaskFlow.IntegrationTests.TestData.Persistence;
using Xunit;

namespace TaskFlow.IntegrationTests.Projects.Commands;

[Collection("IntegrationTests")]
public class UpdateProjectEndpointTests : IntegrationTestBase
{
    public UpdateProjectEndpointTests(TaskFlowApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Update_Project_AsAdmin_UpdatesNameAndDescription_AndReturnsNoContent()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders("Admin");

        var project = new ProjectBuilder()
            .WithName("Super Project")
            .WithDescription("Super Description")
            .Build();

        await TestDb.AddAsync(project);

        UpdateProjectRequest request = new UpdateProjectRequest(
            Name: "Updated Project Name",
            Description: "Updated Project Description"
        );

        var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updated = await TestDb.SingleAsync<Project>(
            db => db.Projects.Where(p => p.Id == project.Id)
            );

        updated.Name.Should().Be(request.Name);
        updated.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task Update_Project_WithInvalidRole_ReturnsForbidden()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "Not_Valid_Role");

        UpdateProjectRequest request = new UpdateProjectRequest(
            Name: "Updated Project Name",
            Description: "Updated Project Description"
        );

        var response = await client.PutAsJsonAsync($"/api/projects/{Guid.NewGuid()}", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_Project_WithInvalidProjectId_ReturnsNotFound()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager");

        UpdateProjectRequest request = new UpdateProjectRequest(
            Name: "Updated Project Name",
            Description: "Updated Project Description"
        );

        var response = await client.PutAsJsonAsync($"/api/projects/{Guid.NewGuid()}", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
