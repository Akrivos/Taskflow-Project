using Moq;
using System;
using System.Threading.Tasks;
using TaskFlow.Application.Comments.Commands.DeleteComment;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Domain.Entities;
using Xunit;

namespace TaskFlow.UnitTests.Application.Comments.Commands;

public class DeleteCommentCommandHandlerTests
{
    private readonly Mock<ICommentReadRepository> _mockCommentReadRepository = new();
    private readonly Mock<ICommentWriteRepository> _mockCommentRepository = new();
    private readonly Mock<ICurrentUser> _mockCurrentUser = new();

    private DeleteCommentCommandHandler CreateHandler() => new DeleteCommentCommandHandler(
        _mockCommentReadRepository.Object,
        _mockCommentRepository.Object,
        _mockCurrentUser.Object);


    [Fact]
    public async Task Handle_WhenUserIsNotAuthenticated_ThrowsForbiddenException()
    {
        var cmd = new DeleteCommentCommand(Guid.NewGuid());
        _mockCurrentUser.Setup(cu => cu.UserId).Returns((string)null);
        _mockCurrentUser.Setup(cu => cu.IsInRole("Admin")).Returns(true);
        var handler = CreateHandler();
        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(cmd, default));
    }

    [Fact]
    public async Task Handle_WhenUserIsNotAuthorized_ThrowsForbiddenException()
    {
        var cmd = new DeleteCommentCommand(Guid.NewGuid());
        _mockCurrentUser.Setup(cu => cu.UserId).Returns("test-user-id");
        _mockCurrentUser.Setup(cu => cu.IsInRole("RandomRole")).Returns(false);
        var handler = CreateHandler();
        await Assert.ThrowsAsync<ForbiddenException>(() => handler.Handle(cmd, default));
    }

    [Fact]
    public async Task Handle_WhenUserIsAuthorized_DeletesComment()
    {
        var cmd = new DeleteCommentCommand(Guid.NewGuid());
        string userId = "test-user-id";
        _mockCurrentUser.Setup(cu => cu.UserId).Returns(userId);
        _mockCurrentUser.Setup(cu => cu.IsInRole("Admin")).Returns(true);
        var comment = new Comment(Guid.NewGuid(), "Test Comment", userId);
        _mockCommentReadRepository.Setup(rp => rp.GetByIdAsync(cmd.Id, default)).ReturnsAsync(comment);
        var handler = CreateHandler();
        await handler.Handle(cmd, default);
        _mockCommentRepository.Verify(rp => rp.DeleteAsync(comment, default), Times.Once);
        _mockCommentRepository.Verify(rp => rp.SaveChangesAsync(default), Times.Once);
    }

}
