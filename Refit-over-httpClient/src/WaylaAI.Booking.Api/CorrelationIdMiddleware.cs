namespace WaylaAI.Booking.Api
{
    public sealed class CorrelationIdMiddleware 
    {
        public const string CorrelationIdHeader = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var newCorrelationId = Guid.NewGuid().ToString();
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            {
                correlationId = newCorrelationId;
                context.Request.Headers.TryAdd(CorrelationIdHeader, correlationId);
            }
            else
            {
                context.Request.Headers[CorrelationIdHeader] = correlationId;
            }

            context.TraceIdentifier = newCorrelationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeader] = correlationId;
                return Task.CompletedTask;
            });
            await _next(context);
        }
    }
}
