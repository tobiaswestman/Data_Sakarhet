using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Server_API.Helpers.Jwt;
using Server_API.Helpers.Interfaces;
using Server_API.Helpers.Services;
using Server_API.Helpers.Repositories;
using Server_API.Helpers.Hubs;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Server_API.Contexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR(); // Lägg till SignalR-tj�nster

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Databases
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration["DataDB"]));
#endregion

#region Helpers
builder.Services.AddScoped<JwtToken>();
builder.Services.AddScoped<ITemperatureDataService, TemperatureDataService>();
builder.Services.AddScoped<IAccountService, AccountService>();
#endregion

#region Repositories
builder.Services.AddScoped<AccountRepo>();
#endregion

#region Identity
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
            if (string.IsNullOrEmpty(context?.Principal?.FindFirst("id")?.Value) || string.IsNullOrEmpty(context?.Principal?.Identity?.Name))
                context?.Fail("Unauthorized");

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting(); // Se till att du har detta innan UseAuthorization
app.UseAuthorization();

app.MapControllers();


app.Run();
