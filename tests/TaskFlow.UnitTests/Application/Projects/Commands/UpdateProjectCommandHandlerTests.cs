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

namespace TaskFlow.UnitTests.Application.Projects.Commands
{
    public class UpdateProjectCommandHandlerTests
    {
        private readonly Mock<IProjectReadRepository> _projectReadRepoMock = new();
        private readonly Mock<IProjectWriteRepository> _projectWriteRepoMock = new();
        private readonly Mock<IQueueService> _queueServiceMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();

        private UpdateProjectCommandHandler CreateHandler()
        {
            return new UpdateProjectCommandHandler(
                _currentUserMock.Object,
                _projectWriteRepoMock.Object,
                _projectReadRepoMock.Object,
                _queueServiceMock.Object);       
        }

        [Fact]
        public async Task Handle_WhenUserIsAuthorized_ReturnsUpdatedProject()
        {
            var cmd = new UpdateProjectCommand(Guid.NewGuid(), "Updated Project", "Updated Description");

            var handler = CreateHandler();
            _currentUserMock.Setup(cu => cu.UserId).Returns(Guid.NewGuid().ToString());
            _currentUserMock.Setup(cu => cu.IsInRole("Admin")).Returns(true);
            _currentUserMock.Setup(cu => cu.IsInRole("ProjectManager")).Returns(false);

            var project = new Project("Old Project", "Old Description");
            project.Id = cmd.Id;
            _projectReadRepoMock.Setup(p => p.GetByIdAsync(It.Is<Guid>(arg => arg == cmd.Id),  It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.Equal(cmd.Id, result);
            _projectWriteRepoMock.Verify(p => p.SaveChangesAsync(CancellationToken.None), Times.Once);

            _queueServiceMock.Verify(
            q => q.PublishAsync(
                Topics.ProjectUpdated,
                It.Is<string>(payload => payload.Contains(cmd.Name)),
                CancellationToken.None),
            Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAuthorized_ThrowsForbiddenException()
        {
           var cmd = new UpdateProjectCommand(Guid.NewGuid(), "Updated Project", "Updated Description");
           var handler = CreateHandler();
           _currentUserMock.Setup(cu => cu.UserId).Returns(Guid.NewGuid().ToString());
           _currentUserMock.Setup(cu => cu.IsInRole("Admin")).Returns(false);
           _currentUserMock.Setup(cu => cu.IsInRole("ProjectManager")).Returns(false);
            await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(cmd, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenProjectDoesNotExists_ThrowsNotFoundException()
        {
            var cmd = new UpdateProjectCommand(Guid.NewGuid(), "Updated Project", "Updated Description");
            var handler = CreateHandler();
            _currentUserMock.Setup(cu => cu.UserId).Returns(Guid.NewGuid().ToString());
            _currentUserMock.Setup(cu => cu.IsInRole("Admin")).Returns(true);
            _currentUserMock.Setup(cu => cu.IsInRole("ProjectManager")).Returns(false);
            _projectReadRepoMock.Setup(p => p.GetByIdAsync(It.Is<Guid>(arg => arg == cmd.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Project?)null);
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(cmd, CancellationToken.None));

            _projectWriteRepoMock.Verify(p => p.SaveChangesAsync(CancellationToken.None), Times.Never);
        }
    }
}
