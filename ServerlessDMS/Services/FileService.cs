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
    public class FileService
    {
        private readonly AuthenticationService _authenticationService;
        private readonly string _endpoint;

        public FileService(AuthenticationService authenticationService, string endpoint)
        {
            _authenticationService = authenticationService;
            _endpoint = endpoint;
        }

        public async Task<byte[]> ConvertFileAsync(string path, string fileId, string targetFormat)
        {
            var token = await _authenticationService.GetAccessTokenAsync();

            var requestUrl = $"{path}{fileId}/content?format={targetFormat}";
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await httpClient.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var fileContent = await response.Content.ReadAsByteArrayAsync();
                return fileContent;
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed with status {response.StatusCode} and message {message}");
            }
        }

        public async Task<DriveItem> UploadFileAsync(string requestUrl, string contentType, byte[] content)
        {
            var token = await _authenticationService.GetAccessTokenAsync();

            var requestContent = new ByteArrayContent(content);
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await httpClient.PutAsync(requestUrl, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var file = JsonConvert.DeserializeObject<DriveItem>(await response.Content.ReadAsStringAsync());
                return file;
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new Exception($"Upload failed with status {response.StatusCode} and message {message}");
            }
        }
    }
}
