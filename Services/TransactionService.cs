using System.Threading.Tasks;
using Identity.Data.Dtos.Response;
using Identity.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Identity.Services
{
    public class TransactionsService : ITransactionService
    {
        private readonly IHttpDataClient _httpDataClient;
        private readonly IConfiguration _configuration;
        public TransactionsService(IHttpDataClient httpDataClient, IConfiguration configuration)
        {
            _httpDataClient = httpDataClient;
            _configuration = configuration;
        }

        public async Task<int> Get(string id)
        {
            int count = 0;
            string url = _configuration["TransactionBaseUrl"] + $"Transaction/count?customerId={id}";
            BaseResponse<int> response = await _httpDataClient.MakeRequest<BaseResponse<int>>(url);
            if (response != null)
                count = response.Data;
            return count;
        }
    }
}