using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Products.Api.Endpoints;
using Products.Api.Endpoints.Validators;
using Products.Api.Infrastructure.Data;
using Products.Api.Infrastructure.Errors;

var builder = WebApplication.CreateBuilder(args);

// Register OpenAPI Services
builder.Services.AddEndpointsApiExplorer();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5000",
                "http://localhost:5500",
                "http://127.0.0.1:5500",
                "http://localhost:8080",
                "http://localhost:3000",
                "https://localhost:5001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth0:Domain"];
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddAuthorization();

// Register EF Core DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ProductsDb"));

// Register FluentValidation from Assembly
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();

// Register IExceptionHandler for RFC 7807 problem details
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Activate Global Exception Handler Middleware
app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Seed In-Memory database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Map Minimal API Endpoints
app.MapProductEndpoints();

app.Run();
