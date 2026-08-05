using Auth0.AuthenticationApi;
using Microsoft.Extensions.Configuration;

namespace WaylaAI.Booking.Infrastructure.Services
{
    public sealed class Auth0TokenProvider : IAuth0TokenProvider
    {
        private readonly IConfiguration _configuration;

        public Auth0TokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetTokenAsync()
        {
            var clientId = _configuration["Auth0:ClientId"];
            var clientSecret = _configuration["Auth0:ClientSecret"];
            var audience = _configuration["Auth0:PaymentApiAudience"];
            var domain = _configuration["Auth0:Domain"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(domain))
            {
                throw new InvalidOperationException("Auth0 configuration is missing.");
            }

            var client = new AuthenticationApiClient(new Uri($"https://{domain}"));

            var tokenRequest = new Auth0.AuthenticationApi.Models.ClientCredentialsTokenRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Audience = audience
            };
            var tokenResponse = await client.GetTokenAsync(tokenRequest);
            return tokenResponse.AccessToken;
        }

    }

    public interface IAuth0TokenProvider
    {
        Task<string> GetTokenAsync();
    }
}
