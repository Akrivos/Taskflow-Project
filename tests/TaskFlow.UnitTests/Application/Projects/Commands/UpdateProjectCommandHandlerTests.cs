using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Projects.Commands;
using TaskFlow.Domain.Entities;
using Xunit;

namespace TaskFlow.UnitTests.Application.Projects.Commands
{
    public class UpdateProjectCommandHandlerTests
    {
        private readonly Mock<IProjectRepository> _projectRepoMock = new();
        private readonly Mock<IQueueService> _queueServiceMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();

        private UpdateProjectCommandHandler CreateHandler()
        {
            return new UpdateProjectCommandHandler(
                _currentUserMock.Object,
                _projectRepoMock.Object,
                _queueServiceMock.Object);       
        }

        [Fact]
        public async Task Handler_Should_Be_Update_Project_When_User_Is_Admin()
        {
            var cmd = new UpdateProjectCommand(Guid.NewGuid(), "Updated Project", "Updated Description");

            var handler = CreateHandler();
            _currentUserMock.Setup(cu => cu.UserId).Returns(Guid.NewGuid().ToString());
            _currentUserMock.Setup(cu => cu.IsInRole("Admin")).Returns(true);
            _currentUserMock.Setup(cu => cu.IsInRole("ProjectManager")).Returns(false);

            var project = new Project("Old Project", "Old Description");
            project.Id = cmd.Id;
            _projectRepoMock.Setup(p => p.GetByIdAsync(It.Is<Guid>(arg => arg == cmd.Id),  It.IsAny<CancellationToken>()))
                .ReturnsAsync(project);

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.Equal(cmd.Id, result);
            _projectRepoMock.Verify(p => p.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handler_Should_Throw_Forbidden_When_User_Role_Is_Not_Admin_Or_ProjectManager()
        {
           var cmd = new UpdateProjectCommand(Guid.NewGuid(), "Updated Project", "Updated Description");
           var handler = CreateHandler();
           _currentUserMock.Setup(cu => cu.UserId).Returns(Guid.NewGuid().ToString());
           _currentUserMock.Setup(cu => cu.IsInRole("Admin")).Returns(false);
           _currentUserMock.Setup(cu => cu.IsInRole("ProjectManager")).Returns(false);
            await Assert.ThrowsAsync<ForbiddenAccessException>(() => handler.Handle(cmd, CancellationToken.None));
        }
    }
}
