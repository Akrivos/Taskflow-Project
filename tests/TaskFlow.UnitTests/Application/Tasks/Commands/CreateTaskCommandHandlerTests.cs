using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Messages;
using TaskFlow.Application.Tasks.Commands;
using TaskFlow.Domain.Entities;
using Xunit;

namespace TaskFlow.UnitTests.Application.Tasks.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskWriteRepository> _taskWriteRepoMock = new();
    private readonly Mock<IQueueService> _queueServiceMock = new();


    private CreateTaskCommandHandler CreateHandler()
        => new(_taskWriteRepoMock.Object, _queueServiceMock.Object);

    [Fact]
    public async Task Handle_WhenUserIsAuthenticated_CreatesTask()
    {
        var projectId = Guid.NewGuid();
        var cmd = new CreateTaskCommand("New Task", "Task Description", projectId);

        TaskItem taskItem = new TaskItem(cmd.Title, cmd.Description, cmd.ProjectId);

        _taskWriteRepoMock
            .Setup(r => r.AddAsync(taskItem, It.IsAny<CancellationToken>()));

        var handler = CreateHandler();
        var resultId = await handler.Handle(cmd, CancellationToken.None);

        resultId.Should().NotBe(Guid.Empty);

        _taskWriteRepoMock.Verify(r => r.AddAsync(
            It.Is<TaskItem>(t => 
                t.Title == cmd.Title && 
                t.Description == cmd.Description && 
                t.ProjectId == cmd.ProjectId),
            It.IsAny<CancellationToken>()), 
            Times.Once);
        _taskWriteRepoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _queueServiceMock.Verify(
            q => q.PublishAsync(
                Topics.TaskCreated,
                It.Is<string>(payload => payload.Contains(cmd.Title)),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }
}
