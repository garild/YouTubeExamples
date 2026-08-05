using MediatR;
using WaylaAI.Payment.Api.Infrastructure;

namespace WaylaAI.Payment.Api.Application.ProcessPayment;

public sealed record ProcessPaymentCommand(string BookingId, string UserId, decimal Amount) : IRequest<ProcessPaymentResult>;

public sealed record ProcessPaymentResult(Guid PaymentId, string Status);

public sealed class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, ProcessPaymentResult>
{
    private readonly PaymentDbContext _dbContext;

    public ProcessPaymentHandler(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProcessPaymentResult> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Domain.Payment.Create(request.BookingId, request.UserId, request.Amount);

        _dbContext.Payments.Add(payment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ProcessPaymentResult(payment.Id, payment.Status);
    }
}
