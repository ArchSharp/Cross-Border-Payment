using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Data.Dtos.Request.Auth;
using Identity.Data.Dtos.Request.User;
using Identity.Data.Dtos.Response.User;
using Identity.Data.Models;

namespace Identity.Interfaces
{
    public interface IStaffService
    {
        Task<Staff> CreateStaff(CreateStaff createStaff);
        Task<bool> Exist(string email);
        Task<Tuple<List<Staff>, int>> Get(int pageNumber, int pageSize, StaffFilter filter);
        Task<List<Role>> Role();
        Task<Role> Role(string roleId);
        Task<Role> AssignPermmission(string roleId, List<Guid> permissions);
        Task<List<Permission>> Pemissions();
    }
}