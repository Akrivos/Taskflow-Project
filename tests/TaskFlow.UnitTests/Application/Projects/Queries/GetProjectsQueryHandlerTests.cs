using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common.Interfaces;
using TaskFlow.Application.Common.Models;
using TaskFlow.Application.Projects.Queries;
using TaskFlow.Application.Projects.Queries.GetProjects;
using Xunit;

namespace TaskFlow.UnitTests.Application.Projects.Queries;

public class GetProjectsQueryHandlerTests
{
    private readonly Mock<IProjectReadRepository> _projectReadRepoMock = new();
  
    private GetProjectsQueryHandler CreateHandler()
        => new(_projectReadRepoMock.Object);

    [Fact]
    public async Task Handle_WhenUserIsAuthenticated_ReturnsPaginatedProjects()
    {
        var projectSortBy = ProjectSortBy.Name;
        var sortDirection = SortDirection.Asc;
        const int pageNumber = 1;
        const int pageSize = 10;

        var query = new GetProjectsQuery(pageNumber, pageSize, null, projectSortBy, sortDirection);

        List<ProjectListItem> items = new List<ProjectListItem>
        {
            new ProjectListItem(Guid.NewGuid(), "My First Project", "My First Description" ),
            new ProjectListItem(Guid.NewGuid(), "Another Project", "Another Description")
        };

        var pagedResult = new PagedResult<ProjectListItem>(
            items,
            pageNumber,
            pageSize,
            2
         );

        _projectReadRepoMock
            .Setup(r => r.GetProjectsAsync(pageNumber, pageSize, null, projectSortBy, sortDirection, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var handler = CreateHandler();
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Collection(result.Items,
            item => Assert.Equal("My First Project", item.Name),
            item => Assert.Equal("Another Project", item.Name));

        _projectReadRepoMock.Verify(r => r.GetProjectsAsync(pageNumber, pageSize, null, projectSortBy, sortDirection, It.IsAny<CancellationToken>()), Times.Once);
        _projectReadRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenRequestingSecondPage_ReturnsCorrectProjects()
    {
        var projectSortBy = ProjectSortBy.Name;
        var sortDirection = SortDirection.Asc;
        const int pageNumber = 2;
        const int pageSize = 2;

        var query = new GetProjectsQuery(
            pageNumber,
            pageSize,
            null,
            projectSortBy,
            sortDirection);

        List<ProjectListItem> items = new List<ProjectListItem>
        {
            new ProjectListItem(Guid.NewGuid(), "Project C", "Desc C"),
            new ProjectListItem(Guid.NewGuid(), "Project D", "Desc D")
        };

        var pagedResult = new PagedResult<ProjectListItem>(
            items,
            pageNumber,
            pageSize,
            totalCount: 4);

        _projectReadRepoMock
            .Setup(r => r.GetProjectsAsync(
                pageNumber,
                pageSize,
                null,
                projectSortBy,
                sortDirection,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var handler = CreateHandler();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(4, result.TotalCount);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(2, result.Items.Count);

        Assert.Collection(result.Items,
            first => Assert.Equal("Project C", first.Name),
            second => Assert.Equal("Project D", second.Name));

        _projectReadRepoMock.Verify(r => r.GetProjectsAsync(
            pageNumber,
            pageSize,
            null,
            projectSortBy,
            sortDirection,
            It.IsAny<CancellationToken>()),
            Times.Once);

        _projectReadRepoMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenSearchingByName_ReturnsMatchingProjects()
    {
        var projectSortBy = ProjectSortBy.Name;
        var sortDirection = SortDirection.Asc;
        const int pageNumber = 1;
        const int pageSize = 10;
        const string searchTerm = "Api";

        var query = new GetProjectsQuery(
            pageNumber,
            pageSize,
            searchTerm,
            projectSortBy,
            sortDirection);

        List<ProjectListItem> items = new List<ProjectListItem>
        {
            new ProjectListItem(Guid.NewGuid(), "Api Gateway", "Desc C"),
            new ProjectListItem(Guid.NewGuid(), "Api Management", "Desc D")
        };

        var pagedResult = new PagedResult<ProjectListItem>(
            items,
            pageNumber,
            pageSize,
            totalCount: 2);

        _projectReadRepoMock
            .Setup(r => r.GetProjectsAsync(
                pageNumber,
                pageSize,
                searchTerm,
                projectSortBy,
                sortDirection,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var handler = CreateHandler();
        var result = await handler.Handle(query, CancellationToken.None);

      
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, item =>
        Assert.Contains(searchTerm, item.Name, StringComparison.OrdinalIgnoreCase));

        _projectReadRepoMock.Verify(r => r.GetProjectsAsync(
            pageNumber,
            pageSize,
            searchTerm,
            projectSortBy,
            sortDirection,
            It.IsAny<CancellationToken>()),
            Times.Once);

        _projectReadRepoMock.VerifyNoOtherCalls();
    }
}
