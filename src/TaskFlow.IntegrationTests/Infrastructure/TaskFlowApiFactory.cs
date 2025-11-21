//using System.Linq;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using TaskFlow.Infrastructure.Persistence;
//using TaskFlow.IntegrationTests.Infrastructure;

//namespace TaskFlow.IntegrationTests.Infrastructure;

//// Σηκώνει το πραγματικό API, αλλά:
//// - Χρησιμοποιεί InMemory DB
//// - Χρησιμοποιεί TestAuth (αντί για JWT)
//public class TaskFlowApiFactory : WebApplicationFactory<Program>
//{
//    protected override IHost CreateHost(IHostBuilder builder)
//    {
//        builder.UseEnvironment("Testing");

//        builder.ConfigureServices(services =>
//        {
//            // 1. Αντικατάσταση DB με InMemory
//            var descriptor = services.SingleOrDefault(
//                d => d.ServiceType == typeof(DbContextOptions<TaskFlowDbContext>));

//            if (descriptor != null)
//            {
//                services.Remove(descriptor);
//            }

//            services.AddDbContext<TaskFlowDbContext>(options =>
//            {
//                options.UseInMemoryDatabase("TaskFlowTestDb");
//            });

//            // 2. Προσθήκη fake authentication scheme
//            services.AddAuthentication(options =>
//            {
//                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
//                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
//            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
//                TestAuthHandler.SchemeName, _ => { });
//        });

//        return base.CreateHost(builder);
//    }
//}
