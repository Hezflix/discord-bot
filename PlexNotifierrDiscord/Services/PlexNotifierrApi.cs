using Microsoft.Extensions.Options;
using PlexNotifierrDiscord.Models;
using System.Text;
using System.Text.Json;

namespace PlexNotifierrDiscord.Services
{
    public class PlexNotifierrApi : IPlexNotifierrApi
    {
        private readonly string _plexNotifierrApiUrl;

        public PlexNotifierrApi(IOptions<PlexNotifierrApiConfig> config)
        {
            _plexNotifierrApiUrl = config.Value.Hostname;
        }

        public async Task<bool> Subscribe(ulong discordId, string plexUsername)
        {
            var bodyContent = $"{{\"discordId\": \"{discordId}\", \"plexName\":\"{plexUsername}\"}}";
            return await SendApiCall(bodyContent, HttpMethod.Post);
;        }

        public async Task<bool> Unsubscribe(ulong discordId)
        {
            var bodyContent = $"{{\"discordId\": \"{discordId}\"}}";
            return await SendApiCall(bodyContent, HttpMethod.Delete);
        }

        private async Task<bool> SendApiCall(string bodyContent, HttpMethod method)
        {
            using var client = new HttpClient();
            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = method;
            requestMessage.RequestUri = new Uri($"{_plexNotifierrApiUrl}/subscription");
            requestMessage.Content = new StringContent(bodyContent, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(requestMessage);
            return response.IsSuccessStatusCode;
        }
    }
}