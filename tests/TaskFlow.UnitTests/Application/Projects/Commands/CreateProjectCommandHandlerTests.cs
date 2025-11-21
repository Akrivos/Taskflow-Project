using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Projects.Commands;
using TaskFlow.Domain.Entities;
using Xunit;

namespace TaskFlow.UnitTests.Application.Projects.Commands;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<IQueueService> _queueServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private CreateProjectCommandHandler CreateHandler()
        => new(_currentUserMock.Object, _projectRepoMock.Object, _queueServiceMock.Object);

    [Fact]
    public async Task Handle_Should_Create_Project_When_User_Is_Admin()
    {
        var cmd = new CreateProjectCommand("Test project", "Some description");
        var ct = CancellationToken.None;

        _currentUserMock.Setup(x => x.UserId).Returns("test-user-id");
        _currentUserMock.Setup(x => x.IsInRole("Admin")).Returns(true);
        _currentUserMock.Setup(x => x.IsInRole("ProjectManager")).Returns(false);

        var handler = CreateHandler();

        var resultId = await handler.Handle(cmd, ct);

        resultId.Should().NotBe(Guid.Empty);

        _projectRepoMock.Verify(
            r => r.AddAsync(
                It.Is<Project>(p => p.Name == cmd.Name && p.Description == cmd.Description),
                ct),
            Times.Once);

        _projectRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);

        _queueServiceMock.Verify(
            q => q.PublishAsync(
                "project-created",
                It.Is<string>(payload => payload.Contains(cmd.Name)),
                ct),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Forbidden_When_User_Has_No_Role()
    {
        var cmd = new CreateProjectCommand("Test project", "Some description");
        var ct = CancellationToken.None;

        _currentUserMock.Setup(x => x.UserId).Returns((string?)null);
        _currentUserMock.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);

        var handler = CreateHandler();

        var act = async () => await handler.Handle(cmd, ct);

        await act.Should()
            .ThrowAsync<ForbiddenAccessException>()
            .WithMessage("You dont have access!");

        _projectRepoMock.Verify(r => r.AddAsync(It.IsAny<Project>(), ct), Times.Never);
        _queueServiceMock.Verify(q => q.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), ct), Times.Never);
    }
}
