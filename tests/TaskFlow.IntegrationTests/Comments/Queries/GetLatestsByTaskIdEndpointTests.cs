using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
using TaskFlow.IntegrationTests.Infrastructure;
using TaskFlow.IntegrationTests.TestData.Builders;
using Xunit;

namespace TaskFlow.IntegrationTests.Comments.Queries;

[Collection("IntegrationTests")]
public class GetLatestsByTaskIdEndpointTests : IntegrationTestBase
{
    public GetLatestsByTaskIdEndpointTests(TaskFlowApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_Latest_Comments_By_TaskId_AsUser_ReturnsOk()
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

        await SeedCommentsAsync(taskId: task.Id, userId: user.Id, count: 10);

        var client = CreateClientWithHeaders(role: "User", id: user.Id, username: user.UserName!);

        var response = await client.GetAsync($"/api/comments/task/{task.Id}/latests?limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var comments = await response.Content.ReadFromJsonAsync<IReadOnlyList<LatestCommentItem>>();
        comments.Should().NotBeNull();
        comments!.Should().HaveCount(5);
    }

    private async Task SeedCommentsAsync(Guid taskId, string userId, int count)
    {
        for (var i = 1; i <= count; i++)
        {
            var comment = new CommentBuilder()
                .ForTask(taskId)
                .ByUser(userId)
                .WithContent($"Content {i}")
                .Build();

            await TestDb.AddAsync(comment);
        }
    }
}
