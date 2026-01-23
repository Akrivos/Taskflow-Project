using FluentAssertions;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.IntegrationTests.Infrastructure;
using TaskFlow.IntegrationTests.TestData.Builders;
using Xunit;

namespace TaskFlow.IntegrationTests.Comments.Commands;

[Collection("IntegrationTests")]
public class CreateCommentEndpointTests : IntegrationTestBase
{
    public CreateCommentEndpointTests(TaskFlowApiFactory factory) : base(factory) { }

    private sealed record CreateCommentRequest(Guid TaskItemId, string Content);

    [Fact]
    public async Task Create_Comment_AsUser_ReturnsCreatedResponse()
    {
        await Factory.ResetDatabaseAsync();

        var project = new ProjectBuilder()
            .WithName("Project Name 1")
            .WithDescription("Project Descr 1")
            .Build();
        await TestDb.AddAsync(project);

        var task = new TaskItemBuilder()
            .ForProject(project.Id)
            .WithTitle("Task 1")
            .WithDescription("Task Description")
            .Build();
        await TestDb.AddAsync(task);

        var user = new UserBuilder()
            .WithUserName("test-user")
            .WithEmail("test-user@test")
            .Build();
        await TestDb.AddAsync(user);

        var client = CreateClientWithHeaders(role: "User", id: user.Id, username: user.UserName!);

        CreateCommentRequest request = new CreateCommentRequest(
            TaskItemId: task.Id,
            Content: "This is a test comment."
        );

        var response = await client.PostAsJsonAsync("/api/comments", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_Comment_AsUser_WithInvalidTaskId_ReturnsNotFound()
    {
        await Factory.ResetDatabaseAsync();

        var user = new UserBuilder()
            .WithUserName("test-user")
            .WithEmail("test-user@test")
            .Build();
        await TestDb.AddAsync(user);

        var client = CreateClientWithHeaders(role: "User", id: user.Id, username: user.UserName!);

        CreateCommentRequest request = new CreateCommentRequest(
            TaskItemId: Guid.NewGuid(),
            Content: "This is a test comment."
        );

        var response = await client.PostAsJsonAsync("/api/comments", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
