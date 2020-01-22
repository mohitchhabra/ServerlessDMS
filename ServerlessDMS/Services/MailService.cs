using Newtonsoft.Json;
using ServerlessDMS.Authentication;
using ServerlessDMS.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessDMS.Services
{
    public class MailService
    {
        private readonly AuthenticationService _authenticationService;
        private readonly string _endpoint;

        public MailService(AuthenticationService authenticationService, string endpoint)
        {
            _authenticationService = authenticationService;
            _endpoint = endpoint;
        }

        public async Task<IEnumerable<Attachment>> GetAttachmentByMailResourceAsync(string resource)
        {
            var token = await _authenticationService.GetAccessTokenAsync();
            var requestUrl = $"{_endpoint}{resource}/attachments";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var attachments = JsonConvert.DeserializeObject<GraphResult<IEnumerable<Attachment>>>(responseContent);
                return attachments.Value;
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }
    }
}
