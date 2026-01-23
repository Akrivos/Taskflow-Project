using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Exceptions;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Projects.Queries;
using TaskFlow.Domain.Entities;
using Xunit;

namespace TaskFlow.UnitTests.Application.Projects.Queries;

public class GetProjectQueryHandlerTests
{
    private readonly Mock<IProjectReadRepository> _projectReadRepoMock = new();

    private GetProjectQueryHandler CreateHandler()
        => new(_projectReadRepoMock.Object);

    [Fact]
    public async Task Handle_WhenProjectExists_ReturnsProjectDto()
    {
        var projectId = Guid.NewGuid();
        var query = new GetProjectQuery(projectId);

        var expectedProject = new Project("Test Project", "Test Description")
        {
            Id = projectId
        };

        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        _projectReadRepoMock
            .Setup(r => r.GetByIdAsync(projectId, token))
            .ReturnsAsync(expectedProject);

        var handler = CreateHandler();

        var result = await handler.Handle(query, token);

        Assert.Equal(projectId, result.Id);
        Assert.Equal("Test Project", result.Name);
        Assert.Equal("Test Description", result.Description);

        _projectReadRepoMock.Verify(r => r.GetByIdAsync(projectId, token), Times.Once);
        _projectReadRepoMock.VerifyNoOtherCalls();
    }


    [Fact]
    public async Task Handle_WhenProjectNotExists_ThrowsNotFoundException()
    {
        var projectId = Guid.NewGuid();
        var query = new GetProjectQuery(projectId);

        _projectReadRepoMock
            .Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var handler = CreateHandler();
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(query, CancellationToken.None));

        _projectReadRepoMock.Verify(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
