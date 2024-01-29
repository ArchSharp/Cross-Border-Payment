using System;
using System.Threading.Tasks;
using Identity.Data.Dtos.Response;
using Identity.Data.Dtos.Response.Location;
using Identity.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Identity.Services
{
    public class LocationService : ILocationService
    {
        private readonly IHttpDataClient _httpDataClient;
        private readonly IConfiguration _configuration;

        public LocationService(IHttpDataClient httpDataClient, IConfiguration configuration)
        {
            _httpDataClient = httpDataClient;
            _configuration = configuration;
        }

        public async Task<LocationResponse> Get(string countryName)
        {
            string url = _configuration["LocationBaseUrl"] + $"/country/key/{countryName}";
            BaseResponse<LocationResponse> response = await _httpDataClient.MakeRequest<BaseResponse<LocationResponse>>(url);
            return response?.Data;
        }
    }
}