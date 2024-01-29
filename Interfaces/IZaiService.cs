using System.Threading.Tasks;
using Identity.Data.Dtos.Request.Zai;

namespace Identity.Interfaces
{
    public interface IZaiService
    {
        Task<bool> Create(CreateZaiUser user);
    }
}