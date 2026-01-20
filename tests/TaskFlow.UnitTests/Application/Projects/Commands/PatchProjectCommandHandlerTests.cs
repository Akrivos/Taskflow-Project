using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Messages;
using TaskFlow.Application.Projects.Commands;
using TaskFlow.Domain.Entities;
using Xunit;

namespace TaskFlow.UnitTests.Application.Projects.Commands;

public class PatchProjectCommandHandlerTests
{
    private readonly Mock<IProjectReadRepository> _projectReadRepoMock = new();
    private readonly Mock<IProjectWriteRepository> _projectWriteRepoMock = new();
    private readonly Mock<IQueueService> _queueServiceMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();

    private PatchProjectCommandHandler CreateHandler()
    {
        return new PatchProjectCommandHandler(
            _currentUserMock.Object,
            _projectWriteRepoMock.Object,
            _projectReadRepoMock.Object,
            _queueServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserNotAdminOrPM_ThrowsForbiddenException()
    {
        var cmd = new PatchProjectCommand(Guid.NewGuid(), null, null);
        _currentUserMock.Setup(cu => cu.IsInRole("Admin")).Returns(false);
        _currentUserMock.Setup(cu => cu.IsInRole("ProjectManager")).Returns(false);
        _currentUserMock.Setup(cu => cu.UserId).Returns("test-user-id");

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(cmd, default));
    }

    [Fact]
    public async Task Handle_WhenProjectNotFound_ThrowsNotFoundException()
    {
        var cmd = new PatchProjectCommand(Guid.NewGuid(), null, null);
        _currentUserMock.Setup(cu => cu.IsInRole("ProjectManager")).Returns(true);
        _currentUserMock.Setup(cu => cu.UserId).Returns("test-user-id");
        _projectReadRepoMock.Setup(p => p.GetByIdAsync(cmd.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);
        var handler = CreateHandler();
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(cmd, CancellationToken.None));

        _projectReadRepoMock.Verify(p => p.GetByIdAsync(cmd.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAdmin_UpdatesProject_Saves_AndPublishesEvent()
    {
        var projectId = Guid.NewGuid();
        var cmd = new PatchProjectCommand(projectId, "Patched Project Name", "Patched Project Description");

        var handler = CreateHandler();

        _currentUserMock.Setup(cu => cu.UserId).Returns("test-user-id");
        _currentUserMock.Setup(cu => cu.IsInRole("Admin")).Returns(true);
        _currentUserMock.Setup(cu => cu.IsInRole("ProjectManager")).Returns(false);

        var project = new Project("Old Project", "Old Description");
        project.Id = cmd.Id;

        _projectReadRepoMock.Setup(p => p.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await handler.Handle(cmd, It.IsAny<CancellationToken>());
        Assert.Equal(cmd.Id, result);
        Assert.Equal("Patched Project Name", project.Name);
        Assert.Equal("Patched Project Description", project.Description);

        _queueServiceMock.Verify(
            q => q.PublishAsync(
                Topics.ProjectPartialUpdated,
                It.Is<string>(payload => payload.Contains(cmd.Name)),
                It.IsAny<CancellationToken>()),
                Times.Once);

        _projectWriteRepoMock.Verify(p => p.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
