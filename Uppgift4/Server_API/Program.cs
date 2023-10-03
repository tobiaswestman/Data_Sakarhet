using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Server_API.Helpers.Jwt;
using Server_API.Helpers.Interfaces;
using Server_API.Helpers.Services;
using Server_API.Helpers.Repositories;
using Server_API.Helpers.Hubs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR(); // Lägg till SignalR-tj�nster

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Helpers
builder.Services.AddScoped<ITemperatureDataService, TemperatureDataService>();
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
        ValidIssuer = builder.Configuration["TokenIssuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["TokenAudience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["TokenSecretKey"]!))
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
