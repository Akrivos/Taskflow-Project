using FluentAssertions;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs;
using TaskFlow.IntegrationTests.Infrastructure;
using TaskFlow.IntegrationTests.TestData.Builders;
using Xunit;

namespace TaskFlow.IntegrationTests.Projects.Queries;

[Collection("IntegrationTests")]
public class GetProjectsEndpointTests : IntegrationTestBase
{
    public GetProjectsEndpointTests(TaskFlowApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_Projects_AsProjectManager_ReturnsPaginatedListOfProjects()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager", id: "test-user-id", username: "test-user");

        await SeedProjectsAsync(15);

        var result = await GetProjectsAsync(
            client,
            pageNumber: 1,
            pageSize: 10,
            sortBy: "Name"
        );

        result.Items.Should().HaveCount(10);
        result.TotalCount.Should().Be(15);
    }

    [Fact]
    public async Task Get_Projects_AsProjectManager_ReturnsPaginatedListOfProjects_SecondPage()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager", id: "test-user-id", username: "test-user");

        await SeedProjectsAsync(15);

        var result = await GetProjectsAsync(
            client,
            pageNumber: 2,
            pageSize: 10,
            sortBy: "Name"
        );

        result.Items.Should().HaveCount(5);
        result.TotalCount.Should().Be(15);
    }

    [Fact]
    public async Task Get_Projects_AsProjectManager_ReturnsPaginatedListOfProjects_MatchingSearchWord()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager", id: "test-user-id", username: "test-user");

        await TestDb.AddAsync(new ProjectBuilder().WithName("Super Project").WithDescription("Description for Super").Build());
        await TestDb.AddAsync(new ProjectBuilder().WithName("Another Project").WithDescription("Description for Another").Build());
        await TestDb.AddAsync(new ProjectBuilder().WithName("Third Project").WithDescription("Description for Third").Build());

        var result = await GetProjectsAsync(
            client,
            pageNumber: 1,
            pageSize: 10,
            sortBy: "Name",
            search: "Anoth"
        );

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        result.Items[0].Name.Should().Be("Another Project");
    }

    [Fact]
    public async Task Get_Projects_AsProjectManager_ReturnsPaginatedListOfProjects_SortedByCreationDate_AndDirectionDesc()
    {
        await Factory.ResetDatabaseAsync();
        var client = CreateClientWithHeaders(role: "ProjectManager", id: "test-user-id", username: "test-user");

        await TestDb.AddAsync(new ProjectBuilder().WithName("Project 1").WithDescription("Description 1").Build());
        await TestDb.AddAsync(new ProjectBuilder().WithName("Project 2").WithDescription("Description 2").Build());
        await TestDb.AddAsync(new ProjectBuilder().WithName("Project 3").WithDescription("Description 3").Build());

        var result = await GetProjectsAsync(
            client,
            pageNumber: 1,
            pageSize: 10,
            sortBy: "CreatedAt",
            sortDirection: "Desc"
        );

        result.Items.Should().HaveCount(3);

        result.Items[0].Name.Should().Be("Project 3");
        result.Items[1].Name.Should().Be("Project 2");
        result.Items[2].Name.Should().Be("Project 1");
    }


    private async Task SeedProjectsAsync(int count)
    {
        for (var i = 1; i <= count; i++)
        {
            var project = new ProjectBuilder()
                .WithName($"Project {i}")
                .WithDescription($"Description for project {i}")
                .Build();

            await TestDb.AddAsync(project);
        }
    }

    private static async Task<PagedResult<ProjectResponseDto>> GetProjectsAsync(
        System.Net.Http.HttpClient client,
        int pageNumber,
        int pageSize,
        string sortBy,
        string? sortDirection = null,
        string? search = null)
    {
        var uri = $"/api/projects?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={Uri.EscapeDataString(sortBy)}" +
            (sortDirection is not null ? $"&sortDirection={Uri.EscapeDataString(sortDirection)}" : "") +
            (search is not null ? $"&search={Uri.EscapeDataString(search)}" : "");

        var response = await client.GetAsync(uri);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PagedResult<ProjectResponseDto>>();
        payload.Should().NotBeNull();

        return payload!;
    }
}
