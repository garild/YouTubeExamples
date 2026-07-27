using WaylaAI.Booking.Infrastructure.Services;

public sealed class Auth0TokenHandler : DelegatingHandler
{
    private readonly IAuth0TokenProvider _tokenProvider;
    public Auth0TokenHandler(IAuth0TokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync();

        if(string.IsNullOrEmpty(token))
        {
          throw new InvalidOperationException("Failed to retrieve Auth0 token.");
        }

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}
