using System.Net.Http;
using TaskFlow.IntegrationTests.TestData.Persistence;

namespace TaskFlow.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase
{
    protected readonly TaskFlowApiFactory Factory;
    protected TestDb TestDb { get; }

    protected IntegrationTestBase(TaskFlowApiFactory factory)
    {
        Factory = factory;
        TestDb = new TestDb(factory.Services);
    }

    protected HttpClient CreateClientWithHeaders(string? role = null, string? id = null, string? username = null)
    {
        var client = Factory.CreateClient();

        if (!string.IsNullOrWhiteSpace(role))
        {
            client.DefaultRequestHeaders.Add("X-Test-Role", role);
        }

        if (!string.IsNullOrWhiteSpace(id))
        {
            client.DefaultRequestHeaders.Add("X-Test-User", id);
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            client.DefaultRequestHeaders.Add("X-Test-UserName", username);
        }

        return client;
    }
}