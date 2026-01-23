using FluentAssertions;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.IntegrationTests.Infrastructure;
using TaskFlow.IntegrationTests.TestData.Builders;
using Xunit;

namespace TaskFlow.IntegrationTests.Tasks.Commands;

[Collection("IntegrationTests")]
public class CreateTaskEndpointTests : IntegrationTestBase
{
    public CreateTaskEndpointTests(TaskFlowApiFactory factory) : base(factory) { }
    private sealed record CreateTaskRequest(string Title, string Description, Guid ProjectId);

    [Fact]
    public async Task Create_Task_AsProjectManager_ReturnsCreatedResponse()
    {
        await Factory.ResetDatabaseAsync();

        var project = new ProjectBuilder()
            .WithName("Project Name")
            .WithDescription("Project Description")
            .Build();

        await TestDb.AddAsync(project);

        var client = CreateClientWithHeaders(role: "ProjectManager", id: "test-user-id", username: "test-user");

        var request = new CreateTaskRequest(
            Title: "New Task",
            Description: "Task Description",
            ProjectId: project.Id
        );

        var response = await client.PostAsJsonAsync("/api/tasks", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
