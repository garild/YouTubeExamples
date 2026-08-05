using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using FluentValidation;
using WaylaAI.Payment.Api.Application.ProcessPayment;

namespace WaylaAI.Payment.Api.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments")
            .RequireAuthorization();

        group.MapPost("/", async (
            ISender sender, 
            [FromBody] ProcessPaymentRequest request, 
            ClaimsPrincipal user, 
            IValidator<ProcessPaymentCommand> validator, 
            CancellationToken cancellationToken) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var command = new ProcessPaymentCommand(request.BookingId, userId, request.Amount);
            
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                
                return Results.ValidationProblem(errors);
            }

            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        });
    }
}

public sealed record ProcessPaymentRequest(string BookingId, decimal Amount);
