using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Data.Models;

namespace Identity.Interfaces
{
    public interface IUserService
    {
        Task<bool> Exist(string email);
        Task<Tuple<User, Role>> CreateUser(User user);
        Task<Tuple<User, Role>> CreateStaff(User user, string roleId);
        Task<User> User(string email);
        Task<Role> UserRole(string email);
        Task<bool> HasPin(string email);
        Task<User> Pin(string pin, string email);
        Task<User> UpdatePassword(string password, string email);
        Task AddRefereshToken(User user, string refereshToken);
        Task<bool> CheckRefereshToken(string customerId, string refereshToken);
        Task UpdateProfile(User user);
        Task Update(User user);
        Task<List<User>> Users(int pageNumber, int pageSize, string name, string email, string role);
        Task<List<User>> Users(int pageNumber, int pageSize, string name, string email, Guid role);
        Task<int> Count();
        Task<int> Count(string roleName);
        Task<int> Count(Guid roleId);
        Task<User> Status(string customerId, bool status);
        Task<List<Role>> Roles();
        Task<Role> Role(Guid roleId);
    }
}