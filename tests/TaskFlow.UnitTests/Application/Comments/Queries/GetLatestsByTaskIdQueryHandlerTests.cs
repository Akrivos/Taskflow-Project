using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Comments.Queries.GetLatestsByTaskId;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using Xunit;

namespace TaskFlow.UnitTests.Application.Comments.Queries;

public class GetLatestsByTaskIdQueryHandlerTests
{
    private readonly Mock<ICommentReadRepository> _mockCommentReadRepository = new();

    private GetLatestsByTaskIdQueryHandler CreateHandler() => new GetLatestsByTaskIdQueryHandler(
        _mockCommentReadRepository.Object);

    [Fact]
    public async Task Handle_GivenTaskId_ReturnsLatestCommentsForTask()
    {
        var taskId = Guid.NewGuid();
        var userId = "test-user-id";

        const int take = 10;
        const SortDirection direction = SortDirection.Desc;
        const CommentSortBy sortBy = CommentSortBy.CreatedAt;
        var query = new GetLatestsByTaskIdQuery(taskId, take, direction, sortBy);

        var taskSummary = new TaskSummary(taskId, "Test Task", "This is a test task.");

        var comments = new List<LatestCommentItem>
        {
            new LatestCommentItem(Guid.NewGuid(), "Content 1", DateTimeOffset.UtcNow.AddMinutes(-30), userId, taskSummary),
            new LatestCommentItem(Guid.NewGuid(), "Content 2", DateTimeOffset.UtcNow.AddMinutes(-25), userId, taskSummary),
            new LatestCommentItem(Guid.NewGuid(), "Content 3", DateTimeOffset.UtcNow.AddMinutes(-15), userId, taskSummary),
        };

        _mockCommentReadRepository
            .Setup(rp => rp.GetLatestsByTaskIdAsync(
                taskId,
                take,
                direction,
                sortBy,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(comments.Count, result.Count);
        Assert.Same(comments, result);

        _mockCommentReadRepository.Verify(rp => rp.GetLatestsByTaskIdAsync(
            taskId,
            take,
            direction,
            sortBy,
            It.IsAny<CancellationToken>()), Times.Once);

    }
}
