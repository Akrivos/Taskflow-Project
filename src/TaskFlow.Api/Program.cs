using System.Security.Claims;
using System.Text;

using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

using TaskFlow.Api.Middleware;
using TaskFlow.Api.Services;
using TaskFlow.Application;
using TaskFlow.Infrastructure;
using TaskFlow.Infrastructure.Identity;
using TaskFlow.Infrastructure.Persistence;

// ---------------------------------------------------------
// Builder & configuration
// ---------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// ---------------------------------------------------------
// Logging
// ---------------------------------------------------------
ConfigureLogging(builder);

// ---------------------------------------------------------
// Services (DI)
// ---------------------------------------------------------
ConfigureServices(builder, config);

// ---------------------------------------------------------
// Build app
// ---------------------------------------------------------
var app = builder.Build();

// ---------------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------------
ConfigureMiddleware(app);

// ---------------------------------------------------------
// Seed data (except in Testing Env)
// ---------------------------------------------------------
if (!app.Environment.IsEnvironment("Testing"))
{
    await SeedAsync(app.Services);
}

app.Run();

// ======================================================================
//  Program partial (helpers)
// ======================================================================
public partial class Program
{
    // ---------------------------------------------------------
    //  Logging
    // ---------------------------------------------------------
    private static void ConfigureLogging(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/taskflow-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    // ---------------------------------------------------------
    //  Services (DI)
    // ---------------------------------------------------------
    private static void ConfigureServices(WebApplicationBuilder builder, IConfiguration config)
    {
        var services = builder.Services;

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        // Layer registration
        services.AddApplication();
        services.AddInfrastructure(config);

        // Identity
        services
            .AddIdentityCore<ApplicationUser>(o =>
            {
                o.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TaskFlowDbContext>()
            .AddSignInManager();

        // JWT options & service
        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // Authentication (JWT Bearer)
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                var jwt = config.GetSection("Jwt");

                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt["Key"]!)
                    ),

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,

                    ClockSkew = TimeSpan.Zero
                };
            });

        // Authorization policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            options.AddPolicy("PMOrAdmin", p => p.RequireRole("ProjectManager", "Admin"));
            options.AddPolicy("AnyUser", p => p.RequireRole("User", "ProjectManager", "Admin"));

            options.AddPolicy("Projects.Create", p => p.RequireRole("ProjectManager", "Admin"));
            options.AddPolicy("Projects.Read", p => p.RequireRole("Admin"));

            options.AddPolicy("Tasks.Create", p => p.RequireRole("User", "ProjectManager", "Admin"));
            options.AddPolicy("Tasks.Read", p => p.RequireRole("User", "ProjectManager", "Admin"));

            options.AddPolicy("Files.Upload", p => p.RequireRole("User", "ProjectManager", "Admin"));

            options.AddPolicy("Comments.Create", p => p.RequireRole("User", "ProjectManager", "Admin"));
            options.AddPolicy("Comments.Read", p => p.RequireRole("User", "ProjectManager", "Admin"));
            options.AddPolicy("Comments.Delete", p => p.RequireRole("ProjectManager", "Admin"));
        });

        services.AddControllers();
        services.AddFluentValidationAutoValidation();

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TaskFlow API",
                Version = "v1"
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter: Bearer {your JWT token}"
            };

            c.AddSecurityDefinition("Bearer", scheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { scheme, new List<string>() }
            });
        });

        // Background workers (except Testing Env)
        if (!builder.Environment.IsEnvironment("Testing"))
        {
            services.AddHostedService<TaskFlow.Api.Workers.RabbitMqTaskCreatedConsumer>();
        }

        // Exception Handling Middleware
        services.AddTransient<ExceptionHandlingMiddleware>();
    }

    // ---------------------------------------------------------
    //  Middleware
    // ---------------------------------------------------------
    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }

    // ---------------------------------------------------------
    //  Seed data
    // ---------------------------------------------------------
    private static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();

        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roles = new[] { "Admin", "ProjectManager", "User" };

        foreach (var roleName in roles)
        {
            if (!await roleMgr.RoleExistsAsync(roleName))
            {
                await roleMgr.CreateAsync(new IdentityRole(roleName));
            }
        }

        static async Task EnsureUserAsync(
            UserManager<ApplicationUser> userMgr,
            string username,
            string email,
            string password,
            string role)
        {
            var user = await userMgr.FindByNameAsync(username);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };

                var created = await userMgr.CreateAsync(user, password);
                if (!created.Succeeded)
                {
                    var details = string.Join("; ", created.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create seed user '{username}': {details}");
                }
            }

            if (!await userMgr.IsInRoleAsync(user, role))
            {
                await userMgr.AddToRoleAsync(user, role);
            }
        }

        await EnsureUserAsync(
            userMgr,
            username: "admin",
            email: "admin@test.com",
            password: "Pass@123",
            role: "Admin");

        await EnsureUserAsync(
            userMgr,
            username: "pm",
            email: "pm@test.com",
            password: "Pass@123",
            role: "ProjectManager");

        await EnsureUserAsync(
            userMgr,
            username: "user",
            email: "user@test.com",
            password: "Pass@123",
            role: "User");
    }
}
