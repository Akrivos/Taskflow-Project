using FluentAssertions;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.IntegrationTests.Infrastructure;
using TaskFlow.IntegrationTests.TestData.Builders;
using Xunit;

namespace TaskFlow.IntegrationTests.Projects.Queries;

[Collection("IntegrationTests")]
public class GetProjectEndpointTests : IntegrationTestBase
{
    public GetProjectEndpointTests(TaskFlowApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_Project_AsProjectManager_ReturnsNotFound_ForNonExistentProject()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager", id: "test-user-id", username: "test-user");

        var nonExistentProjectId = Guid.NewGuid();

        var response = await client.GetAsync($"/api/projects/{nonExistentProjectId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_Project_AsProjectManager_ReturnsProjectDetails_ForExistingProject()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager", id: "test-user-id", username: "test-user");

        var projectName = "Existing Project";
        var projectDescription = "This is an existing project descr.";

        var project = new ProjectBuilder()
            .WithName(projectName)
            .WithDescription(projectDescription)
            .Build();

        await TestDb.AddAsync(project);

        var response = await client.GetAsync($"/api/projects/{project.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var projectDetails = await response.Content.ReadFromJsonAsync<Project>();
        projectDetails.Should().NotBeNull();

        projectDetails!.Id.Should().Be(project.Id);
        projectDetails.Name.Should().Be(projectName);
        projectDetails.Description.Should().Be(projectDescription);
    }
}
