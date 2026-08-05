using Payment.Examples.Models;

namespace Payment.Examples.Api;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/payments")
            .WithTags("Payments");

        group.MapPost("/", (PaymentRequest request) =>
        {
            return request switch
            {
                StripePaymentRequest stripe => Results.Ok(new
                {
                    type = "stripe",
                    stripe.Amount,
                    stripe.Currency,
                    stripe.PaymentMethodId
                }),
                PayPalPaymentRequest paypal => Results.Ok(new
                {
                    type = "paypal",
                    paypal.Amount,
                    paypal.Currency,
                    paypal.OrderId
                }),
                _ => Results.BadRequest()
            };
        })
        .WithName("ProcessPayment")
        .WithSummary("Process a payment")
        .WithDescription("Accepts a polymorphic payment request for Stripe or PayPal.")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
