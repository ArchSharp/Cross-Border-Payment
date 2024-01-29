using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Data.Models;

namespace Identity.Interfaces
{
    public interface IRoleService
    {
        Task<Role> Get(string name);
        Task<Role> Get(Guid roleId);
        Task<List<Role>> Get();
    }
}