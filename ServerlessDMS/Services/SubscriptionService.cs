using Newtonsoft.Json;
using ServerlessDMS.Authentication;
using ServerlessDMS.DTO;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessDMS.Services
{
    public class SubscriptionService
    {
        private readonly AuthenticationService _authenticationService;
        private readonly string _endpoint;
        private readonly string _notificationEndpoint;

        public SubscriptionService(AuthenticationService authenticationService, string endpoint, string notificationEndpoint)
        {
            _authenticationService = authenticationService;
            _endpoint = endpoint;
            _notificationEndpoint = notificationEndpoint;
        }

        public async Task<Subscription> CreateAsync(string type, string resource, DateTime expiration)
        {
            var token = await _authenticationService.GetAccessTokenAsync();
            var requestUrl = $"{_endpoint}subscriptions";
            var request = new SubscriptionRequest()
            {
                ChangeType = type,
                ClientState = string.Empty,
                ExpirationDateTime = expiration.ToString("o"),
                NotificationUrl = _notificationEndpoint,
                Resource = resource
            };
            var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.PostAsync(requestUrl, requestBody);
            if (response.IsSuccessStatusCode)
            {
                var subscription = JsonConvert.DeserializeObject<Subscription>(await response.Content.ReadAsStringAsync());
                return subscription;
            }
            else
            {
                var status = response.StatusCode;
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed with status code {status} and message: {message}");
            }
        }
    }
}
