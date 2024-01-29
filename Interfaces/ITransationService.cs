using System.Threading.Tasks;

namespace Identity.Interfaces
{
    public interface ITransactionService
    {
        Task<int> Get(string id);
    }
}