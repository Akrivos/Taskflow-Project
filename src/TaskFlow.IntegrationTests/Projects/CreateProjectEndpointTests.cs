//using System.Net;
//using System.Net.Http.Json;
//using System.Threading.Tasks;
//using FluentAssertions;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using TaskFlow.Infrastructure.Persistence;
//using TaskFlow.IntegrationTests.Infrastructure;
//using Xunit;

//namespace TaskFlow.IntegrationTests.Projects;

//public class CreateProjectEndpointTests : IClassFixture<TaskFlowApiFactory>
//{
//    private readonly TaskFlowApiFactory _factory;

//    public CreateProjectEndpointTests(TaskFlowApiFactory factory)
//    {
//        _factory = factory;
//    }

//    [Fact]
//    public async Task Post_Projects_Should_Create_Project_And_Return_201()
//    {
//        // Arrange
//        var client = _factory.CreateClient(); // ήδη authenticated ως Admin (TestAuth)

//        var request = new
//        {
//            name = "Integration Test Project",
//            description = "Created from integration test"
//        };

//        // Act
//        var response = await client.PostAsJsonAsync("/api/projects", request);

//        // Assert
//        response.StatusCode.Should().Be(HttpStatusCode.Created);

//        // Optional: έλεγξε ότι γράφτηκε στη DB
//        using var scope = _factory.Services.CreateScope();
//        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();

//        var project = await db.Projects.SingleOrDefaultAsync(p => p.Name == request.name);
//        project.Should().NotBeNull();
//        project!.Description.Should().Be(request.description);
//    }
//}
