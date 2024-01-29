using System.Threading.Tasks;
using Identity.Data.Dtos.Response.Location;

namespace Identity.Interfaces
{
    public interface ILocationService
    {
        Task<LocationResponse> Get(string countryName);
    }
}