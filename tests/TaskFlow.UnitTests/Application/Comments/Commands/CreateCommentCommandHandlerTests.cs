using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Comments.Commands.CreateComment;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using Xunit;

namespace TaskFlow.UnitTests.Application.Comments.Commands
{
    public class CreateCommentCommandHandlerTests
    {
        private readonly Mock<ICommentRepository> _mockCommentRepository = new();
        private readonly Mock<ITaskRepository> _mockTaskRepository = new();
        private readonly Mock<ICurrentUser> _mockCurrentUser = new();

        private CreateCommentCommandHandler CreateHandler()  => new CreateCommentCommandHandler(
            _mockCurrentUser.Object,
            _mockCommentRepository.Object,
            _mockTaskRepository.Object);

        [Fact]
        public async Task Handle_Should_Create_Comment_When_User_Is_Authenticated()
        {
            var cmd = new CreateCommentCommand(Guid.NewGuid(), "This is a test comment.");
            _mockCurrentUser.Setup(cu => cu.UserId).Returns("test-user-id");
            var taskItem = new TaskItem("Test Task", "Test Description", Guid.NewGuid());
            taskItem.Id = cmd.TaskItemId;
            _mockTaskRepository.Setup(tr => tr.GetByIdAsync(It.Is<Guid>(arg => arg == cmd.TaskItemId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(taskItem);

            var handler = CreateHandler();
            var result = await handler.Handle(cmd, CancellationToken.None);
            _mockCommentRepository.Verify(cr => cr.AddAsync(It.Is<Comment>(c =>
                c.Content == cmd.Content &&
                c.TaskItemId == cmd.TaskItemId &&
                c.UserId == "test-user-id"
            ), It.IsAny<CancellationToken>()), Times.Once);

            _mockCommentRepository.Verify(cr => cr.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_TaskItem_Does_Not_Exist()
        {
            var cmd = new CreateCommentCommand(Guid.NewGuid(), "This is a test comment.");
            _mockCurrentUser.Setup(cu => cu.UserId).Returns("test-user-id");
            _mockTaskRepository.Setup(tr => tr.GetByIdAsync(It.Is<Guid>(arg => arg == cmd.TaskItemId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem?)null);
            var handler = CreateHandler();
            await Assert.ThrowsAsync<NotFoundException>(() =>
                handler.Handle(cmd, CancellationToken.None));
        }
    }
}
