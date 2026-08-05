using Microsoft.AspNetCore.Authentication.JwtBearer;
using WaylaAI.Booking.Application;
using WaylaAI.Booking.Infrastructure;
using WaylaAI.Booking.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WaylaAI.Booking.Api;
using WaylaAI.Payment.Client;

var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Problem Details
builder.Services.AddProblemDetails();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddPaymentClient<Auth0TokenHandler>("https://localhost:7197");

builder.Services.AddHeaderPropagation(p=> p.Headers.Add(CorrelationIdMiddleware.CorrelationIdHeader));
// Configure Auth0

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = System.Security.Claims.ClaimTypes.NameIdentifier
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WaylaAI.Booking.Infrastructure.Database.BookingDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseExceptionHandler();  
app.UseStatusCodePages();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseHeaderPropagation();
app.MapBookingEndpoints();

app.Run();
