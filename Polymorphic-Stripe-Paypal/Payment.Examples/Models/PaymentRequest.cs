using System.Text.Json.Serialization;

namespace Payment.Examples.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", IgnoreUnrecognizedTypeDiscriminators = false, UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(StripePaymentRequest), "stripe")]
[JsonDerivedType(typeof(PayPalPaymentRequest), "paypal")]
public record PaymentRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

public sealed record StripePaymentRequest : PaymentRequest
{
    public required string PaymentMethodId { get; set; }
}

public sealed record PayPalPaymentRequest : PaymentRequest
{
    public required string OrderId { get; set; }
}
