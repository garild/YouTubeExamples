using MediatR;
using WaylaAI.Booking.Application.DTOs;
using WaylaAI.Booking.Application.UseCases.GetBookings;
using WaylaAI.Booking.Application.UseCases.CreateBooking;
using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace WaylaAI.Booking.Api.Endpoints;

public static class BookingEndpoints
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings")
            .RequireAuthorization();

        group.MapGet("/", async (ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var query = new GetBookingsQuery(userId);
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        });

        group.MapPost("/", async (ISender sender, [FromBody] CreateBookingRequest request, ClaimsPrincipal user, IValidator<CreateBookingCommand> validator, CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var command = new CreateBookingCommand(userId, request.Destination, request.Date, request.Price);
            
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                
                return Results.ValidationProblem(errors);
            }

            var result = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/bookings/{result.Id}", result);
        });
    }
}

public sealed record CreateBookingRequest(string Destination, DateTime Date, decimal Price);
