using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessDMS.Authentication
{
    public class AuthenticationService
    {
        string _tenant, _appId, _appSecret, _scope, _grantType, _resource, _oauthURL;

        public AuthenticationService(string tenant, string clientId, string clientSecret)
        {
            _tenant = tenant;
            _appId = clientId;
            _appSecret = clientSecret;
            Initialize();
        }

        private void Initialize()
        {
            _scope = "https%3A%2F%2Fgraph.microsoft.com%2F.default";
            _grantType = "client_credentials";
            _resource = "https://graph.microsoft.com";
            _oauthURL = "https://login.microsoftonline.com/";
            if (string.IsNullOrEmpty(_tenant) || string.IsNullOrEmpty(_appId) || string.IsNullOrEmpty(_appSecret))
            {
                throw new ArgumentNullException("Tenant or ClientID or ClientSecret.");
            }
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", _appId),
                new KeyValuePair<string, string>("client_secret", _appSecret),
                new KeyValuePair<string, string>("scope", _scope),
                new KeyValuePair<string, string>("grant_type", _grantType),
                new KeyValuePair<string, string>("resource",_resource)
            };
            var client = new HttpClient();
            var requestUrl = $"{_oauthURL}{_tenant}/oauth2/token";
            var requestContent = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(requestUrl, requestContent);
            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
            return tokenResponse?.AccessToken;
        }
    }
}

