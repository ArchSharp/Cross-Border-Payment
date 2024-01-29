using System.Threading.Tasks;
using Identity.Data.Dtos.Request.Zai;
using Identity.Data.Dtos.Response;
using Identity.Data.Dtos.Response.Zai;
using Identity.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Identity.Services
{
    public class ZaiService : IZaiService
    {
        private readonly IHttpDataClient _httpDataClient;
        private readonly IConfiguration _configuration;
        public ZaiService(IHttpDataClient httpDataClient, IConfiguration configuration)
        {
            _httpDataClient = httpDataClient;
            _configuration = configuration;
        }

        public async Task<bool> Create(CreateZaiUser user)
        {
            bool status = false;
            string url = _configuration["ZaiBaseUrl"] + "Zaipay/Create";
            BaseResponse<ZaiAccount> response = await _httpDataClient.MakeRequest<BaseResponse<ZaiAccount>>(user, url);
            if (response != null)
                status = response.Data != null;
            return status;
        }
    }
}