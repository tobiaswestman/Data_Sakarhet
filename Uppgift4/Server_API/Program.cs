using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server_API.Contexts;
using Server_API.Helpers.Hubs;
using Server_API.Helpers.Interfaces;
using Server_API.Helpers.Jwt;
using Server_API.Helpers.Repositories;
using Server_API.Helpers.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR(); // Lägg till SignalR-tj�nster
builder.Services.AddCors();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Databases
// Setup Entity Framework with SQL Server connection string.
builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region Helpers
// Register helper services for dependency injection.
builder.Services.AddScoped<JwtToken>();
builder.Services.AddScoped<ITemperatureDataService, TemperatureDataService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITemperatureDataService, TemperatureDataService>();
builder.Services.AddScoped<IUnitService, UnitService>();
#endregion

#region Repositories
// Register repository services for dependency injection.
builder.Services.AddScoped<AccountRepo>();
builder.Services.AddScoped<TemperatureDataRepo>();
#endregion

#region Authorization Policies
// Define custom authorization policies.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanReadTemperature", policy => policy.RequireClaim("Permission", "Read"));
    options.AddPolicy("CanWriteTemperature", policy => policy.RequireClaim("Permission", "Write"));
});

#endregion

#region Identity
// Configure identity settings for user management.
builder.Services.AddIdentity<IdentityUser, IdentityRole>(x =>
{
    x.Password.RequiredLength = 8;
    x.SignIn.RequireConfirmedAccount = false;
    x.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();


builder.Services.Configure<DataProtectionTokenProviderOptions>(x =>
{
    x.TokenLifespan = TimeSpan.FromHours(10);
});
#endregion

#region Authentication
// Configure JWT authentication.
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            return Task.CompletedTask;
        }
    };

    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["TokenValidation:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["TokenValidation:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["TokenValidation:SecretKey"]!))
    };
});
#endregion

var app = builder.Build();

// Ensure default roles are present in the system.
EnsureRolesCreated(app.Services).Wait();

// Content Security Policy (CSP)
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self';";
    await next();
});


// Set up middleware.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .WithOrigins("https://localhost:7258", "https://localhost:7087")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<TemperatureDataHub>("/temperatureDataHub"); // Map SignalR hub.
app.MapControllers();

app.Run();

// Helper function to ensure necessary roles are created on startup.
static async Task EnsureRolesCreated(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("admin"))
        await roleManager.CreateAsync(new IdentityRole("admin"));

    if (!await roleManager.RoleExistsAsync("user"))
        await roleManager.CreateAsync(new IdentityRole("user"));
}