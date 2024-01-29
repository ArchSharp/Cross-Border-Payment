using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Identity.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Identity.Services
{
    public class HttpDataClient : IHttpDataClient
    {
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public HttpDataClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<T> MakeRequest<T>(string url)
        {
            var httpResponseMessage = await _httpClient.GetAsync(url);
            return await PrepareResponse<T>(httpResponseMessage);
        }

        public async Task<T> MakeRequest<T>(object data, string url)
        {
            string body = System.Text.Json.JsonSerializer
                .Serialize(data, serializeOptions)
                .Replace("\n", "");

            var content = new StringContent(
               body,
               Encoding.UTF8,
               Application.Json
            );

            var httpResponseMessage = await _httpClient.PostAsync(url, content);
            return await PrepareResponse<T>(httpResponseMessage);
        }

        private async Task<T> PrepareResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            T response = default;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                response = System.Text.Json.JsonSerializer.Deserialize<T>(contentStream, serializeOptions);
            }
            return response;
        }
    }
}