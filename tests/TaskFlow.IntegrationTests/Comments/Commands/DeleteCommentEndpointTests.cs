using FluentAssertions;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.IntegrationTests.Infrastructure;
using TaskFlow.IntegrationTests.TestData.Builders;
using Xunit;

namespace TaskFlow.IntegrationTests.Comments.Commands;

[Collection("IntegrationTests")]
public class DeleteCommentEndpointTests : IntegrationTestBase
{
    public DeleteCommentEndpointTests(TaskFlowApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Delete_Comment_AsProjectManager_ReturnsNotFound_ForNonExistentComment()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager", id: "test-user-id", username: "test-user");

        var response = await client.DeleteAsync($"/api/comments/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Comment_AsProjectManager_ReturnsNoContent_ForExistingComment()
    {
        await Factory.ResetDatabaseAsync();

        var project = new ProjectBuilder()
            .WithName("Project Name")
            .WithDescription("Project Description")
            .Build();
        await TestDb.AddAsync(project);

        var task = new TaskItemBuilder()
            .ForProject(project.Id)
            .WithTitle("Task Name")
            .WithDescription("Task Description")
            .Build();
        await TestDb.AddAsync(task);

        var user = new UserBuilder()
            .WithUserName("test-user")
            .WithEmail("test@test.com")
            .Build();
        await TestDb.AddAsync(user);

        var comment = new CommentBuilder()
            .ForTask(task.Id)
            .ByUser(user.Id)
            .WithContent("This is a comment.")
            .Build();
        await TestDb.AddAsync(comment);

        var client = CreateClientWithHeaders(role: "ProjectManager", id: user.Id, username: user.UserName!);

        var response = await client.DeleteAsync($"/api/comments/{comment.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var stillExists = await TestDb.AnyAsync<Comment>(db => db.Comment.Where(c => c.Id == comment.Id));

        stillExists.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_Comment_AsUser_ReturnsForbidden()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "User", id: "test-user-id", username: "test-user");

        var response = await client.DeleteAsync($"/api/comments/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
