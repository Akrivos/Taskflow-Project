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
using Xunit;

namespace TaskFlow.IntegrationTests.Projects.Commands;

[Collection("IntegrationTests")]
public class PatchProjectEndpointTests : IntegrationTestBase
{
    public PatchProjectEndpointTests(TaskFlowApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Patch_Project_AsAdmin_UpdatesName_AndReturnsNoContent()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "Admin");

        var originalName = "Super Project";
        var originalDesc = "Super Description";
        var patchedName = "Patched Project Name";

        var project = new ProjectBuilder()
            .WithName(originalName)
            .WithDescription(originalDesc)
            .Build();

        await TestDb.AddAsync(project);

        PatchProjectRequest request = new PatchProjectRequest(Name: patchedName, Description: null);

        var response = await client.PatchAsJsonAsync($"/api/projects/{project.Id}", request);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updated = await TestDb.SingleAsync<Project>(
            db => db.Projects.Where(p => p.Id == project.Id)
        );

        updated.Name.Should().Be(patchedName);
        updated.Description.Should().Be(originalDesc);
    }

    [Fact]
    public async Task Patch_Project_WithInvalidRole_ReturnsForbidden()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "Not_Valid_Role");

        PatchProjectRequest request = new PatchProjectRequest(Name: "Patched Project Name", Description: null);

        var response = await client.PatchAsJsonAsync(
            $"/api/projects/{Guid.NewGuid()}",
            request
        );

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Patch_Project_WithInvalidProjectId_ReturnsNotFound()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager");

        PatchProjectRequest request = new PatchProjectRequest(Name: "Patched Project Name", Description: null);

        var response = await client.PatchAsJsonAsync(
            $"/api/projects/{Guid.NewGuid()}",
            request
        );

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
