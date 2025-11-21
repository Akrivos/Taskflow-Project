using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;

using TaskFlow.Infrastructure;
using TaskFlow.Infrastructure.Identity;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Application;

using FluentValidation.AspNetCore;
using TaskFlow.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//
// -------------------- Serilog configuration --------------------
// Log to console and also to rolling log files (1 file per day).
//
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/taskflow-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

//
// Expose HttpContext via DI so services can access user info, etc.
//
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

//
// -------------------- Layer registration --------------------
// Application layer (CQRS / MediatR handlers / validators / behaviors).
// Infrastructure layer (DbContext, repositories, RabbitMQ, Blob, etc.).
//
builder.Services.AddApplication();
builder.Services.AddInfrastructure(config);

//
// -------------------- ASP.NET Identity --------------------
// Configure IdentityCore with roles and EF Core stores (TaskFlowDbContext).
// We also add SignInManager so controllers can inject it.
//
builder.Services
    .AddIdentityCore<ApplicationUser>(o =>
    {
        // Require unique email for each registered user.
        o.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()                        // Enable RoleManager / roles
    .AddEntityFrameworkStores<TaskFlowDbContext>()   // Persist Identity to SQL via EF Core
    .AddSignInManager();                             // Register SignInManager<ApplicationUser>

//
// -------------------- JWT options + token service --------------------
// Bind JwtOptions from configuration and register the token generator service.
//
builder.Services.Configure<TaskFlow.Api.Services.JwtOptions>(config.GetSection("Jwt"));
builder.Services.AddSingleton<TaskFlow.Api.Services.IJwtTokenService, TaskFlow.Api.Services.JwtTokenService>();

//
// -------------------- Authentication (JWT Bearer) --------------------
// Configure JWT bearer auth for incoming requests.
// This enables [Authorize] with role checking using JWT tokens we issue.
//
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var jwt = config.GetSection("Jwt");

        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                   // Require correct issuer
            ValidateAudience = true,                 // Require correct audience
            ValidateIssuerSigningKey = true,         // Require valid signing key
            ValidateLifetime = true,                 // Reject expired tokens
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            ),

            // Map the claims in our generated token to what ASP.NET Core expects
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,

            // No extra clock skew on expiration time
            ClockSkew = TimeSpan.Zero
        };
    });

//
// -------------------- Authorization policies --------------------
// Define named policies that require certain roles.
// Controllers can use [Authorize(Policy = "Projects.Create")] etc.
//
builder.Services.AddAuthorization(options =>
{
    // High-level access groupings
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("PMOrAdmin", p => p.RequireRole("ProjectManager", "Admin"));
    options.AddPolicy("AnyUser", p => p.RequireRole("User", "ProjectManager", "Admin"));

    // Feature-level / action-level policies
    options.AddPolicy("Projects.Create", p => p.RequireRole("ProjectManager", "Admin"));
    options.AddPolicy("Projects.Read", p => p.RequireRole("User", "ProjectManager", "Admin"));

    options.AddPolicy("Tasks.Create", p => p.RequireRole("User", "ProjectManager", "Admin"));
    options.AddPolicy("Tasks.Read", p => p.RequireRole("User", "ProjectManager", "Admin"));

    options.AddPolicy("Files.Upload", p => p.RequireRole("User", "ProjectManager", "Admin"));

    options.AddPolicy("Comments.Create", p => p.RequireRole("User", "ProjectManager", "Admin"));
    options.AddPolicy("Comments.Read", p => p.RequireRole("User", "ProjectManager", "Admin"));
    options.AddPolicy("Comments.Delete", p => p.RequireRole("ProjectManager", "Admin"));
});

//
// -------------------- MVC / FluentValidation / Swagger --------------------
// Add controllers, enable automatic validation via FluentValidation,
// and configure Swagger with Bearer auth support.
//
//builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
//{
//    options.SuppressModelStateInvalidFilter = true;
//});
builder.Services.AddControllers();

// Automatically run FluentValidation validators on incoming DTOs.
builder.Services.AddFluentValidationAutoValidation();

// Swagger/OpenAPI so we can test the API easily (and include JWT auth).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskFlow API",
        Version = "v1"
    });

    // Swagger "Authorize" button with Bearer token support
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

//
// -------------------- Background services --------------------
// Attach background RabbitMQ consumer inside the API process.
//
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHostedService<TaskFlow.Api.Workers.RabbitMqTaskCreatedConsumer>();
}

//
// Exception Handling Middleware
//
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

//
// -------------------- Build the app --------------------
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

//
// -------------------- Seed initial data --------------------
// This will ensure default roles and demo users exist in the DB.
//
if (!app.Environment.IsEnvironment("Testing"))
{
    await SeedAsync(app.Services);
}

//
// -------------------- HTTP pipeline --------------------
// Order here matters.
//
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI only in Development environment
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(); // Request logging middleware

app.UseHttpsRedirection();      // Redirect HTTP -> HTTPS (in container it's okay if HTTP-only, but harmless)

app.UseAuthentication();        // Validate JWT and attach User to HttpContext
app.UseAuthorization();         // Enforce [Authorize] attributes / policies

app.MapControllers();           // Map controller routes

app.Run();

//
// -------------------- SeedAsync --------------------
// Creates default roles and a few default users if they do not exist yet.
// This runs at startup, before the app starts serving traffic.
//
static async Task SeedAsync(IServiceProvider sp)
{
    using var scope = sp.CreateScope();

    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1. Ensure required roles exist.
    string[] roles = new[] { "Admin", "ProjectManager", "User" };

    foreach (var roleName in roles)
    {
        if (!await roleMgr.RoleExistsAsync(roleName))
        {
            await roleMgr.CreateAsync(new IdentityRole(roleName));
        }
    }

    // 2. Helper to ensure a user exists with a specific role.
    static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userMgr,
        string username,
        string email,
        string password,
        string role)
    {
        // Try to find the user
        var user = await userMgr.FindByNameAsync(username);

        // If not found, create it
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
                // If creation failed, surface all validation errors here.
                var details = string.Join("; ", created.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create seed user '{username}': {details}");
            }
        }

        // Make sure the user is in the given role
        if (!await userMgr.IsInRoleAsync(user, role))
        {
            await userMgr.AddToRoleAsync(user, role);
        }
    }

    // 3. Create default demo accounts.
    // Admin: full access
    await EnsureUserAsync(userMgr,
        username: "admin",
        email: "admin@example.com",
        password: "Pass@123",
        role: "Admin");

    // Project Manager
    await EnsureUserAsync(userMgr,
        username: "pm",
        email: "pm@example.com",
        password: "Pass@123",
        role: "ProjectManager");

    // Regular User
    await EnsureUserAsync(userMgr,
        username: "user",
        email: "user@example.com",
        password: "Pass@123",
        role: "User");
}

public partial class Program { }
