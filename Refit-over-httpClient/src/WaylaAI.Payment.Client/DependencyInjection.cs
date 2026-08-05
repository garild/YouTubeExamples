using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Text.Json;

namespace WaylaAI.Payment.Client
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPaymentClient<THandler>(this IServiceCollection services, string baseAddress) where THandler : DelegatingHandler
        {
            var settings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    PropertyNameCaseInsensitive = true
                })
            };

            services.AddRefitClient<IPaymentApiClient>(settings).ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(baseAddress);
            })
          .AddHttpMessageHandler<THandler>();

            return services;
        }
    }
}
