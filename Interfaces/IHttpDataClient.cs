using System.Threading.Tasks;

namespace Identity.Interfaces
{
    public interface IHttpDataClient
    {
        Task<T> MakeRequest<T>(string url);
        Task<T> MakeRequest<T>(object data, string url);
    }
}