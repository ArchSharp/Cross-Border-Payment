using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Data.Dtos.Request.Zai;
using Identity.Data.Models;
using Identity.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Identity.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IRoleService _roleService;
        private readonly IZaiService _zaiService;

        public UserService(DataContext context, IRoleService roleService, IZaiService zaiService)
        {
            _context = context;
            _roleService = roleService;
            _zaiService = zaiService;
        }

        public async Task<Tuple<User, Role>> CreateUser(User user)
        {
            Role role = await _roleService.Get("User");
            if (role != null)
            {
                user.CustomerId = "TRT" + DateTime.Now.ToString("yyMMddfff");
                user.RoleId = role.Id;
                user.IsActive = true;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            return new Tuple<User, Role>(user, role);
        }

        public async Task<Tuple<User, Role>> CreateStaff(User user, string roleId)
        {
            Role role = await _roleService.Get(new Guid(roleId));
            if (role != null)
            {
                user.CustomerId = "TRT" + DateTime.Now.ToString("yyMMddfff");
                user.RoleId = role.Id;
                user.IsActive = true;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            return new Tuple<User, Role>(user, role);
        }

        public async Task<Role> UserRole(string email) => await _context.Users
            .Where(u => u.Email.Equals(email))
            .Include(u => u.Role)
            .Select(u => u.Role)
            .FirstOrDefaultAsync();

        public async Task<bool> Exist(string email) => await _context.Users.AnyAsync(u => u.Email.Equals(email));

        public async Task<User> User(string key) => await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(key) || u.CustomerId.Equals(key));

        public async Task<bool> HasPin(string email) => await _context.Users.Where(u => u.Email.Equals(email)).Select(u => u.HasPin).FirstOrDefaultAsync();

        public async Task<User> Pin(string pin, string email)
        {
            var user = await _context.Users.Where(u => u.Email.Equals(email)).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Pin = pin;
                user.HasPin = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return user;
        }

        public async Task<User> UpdatePassword(string password, string email)
        {
            var user = await _context.Users.Where(u => u.Email.Equals(email)).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Password = password;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            return user;
        }

        public async Task AddRefereshToken(User user, string refereshToken)
        {
            var refereshTokens = await _context.UserRefreshTokens.Where(r => r.CustomerId.Equals(user.CustomerId)).ToListAsync();
            _context.RemoveRange(refereshTokens);
            await _context.SaveChangesAsync();

            var newRefereshToken = new UserRefreshToken
            {
                CustomerId = user.CustomerId,
                RefreshToken = refereshToken
            };
            _context.UserRefreshTokens.Add(newRefereshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProfile(User user)
        {
            if (user.Country.Equals("AU") && !user.IsZaiUser)
            {
                var zaiUser = new CreateZaiUser
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Country = user.Country,
                    Mobile = user.PhoneNumber,
                    Id = user.CustomerId
                };
                user.IsZaiUser = await _zaiService.Create(zaiUser);
            }
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckRefereshToken(string customerId, string refereshToken) => await _context.UserRefreshTokens
            .AnyAsync(r => r.RefreshToken.Equals(refereshToken) && r.CustomerId.Equals(customerId));

        public async Task<List<User>> Users(int pageNumber, int pageSize, string name, string email, string roleName)
        {
            var role = await _roleService.Get(roleName);
            var query = _context.Users.Where(x => x.RoleId.Equals(role.Id)).Include(x => x.Role).AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query
                    .Where(u => EF.Functions.Like(u.LastName, $"%{name}%") || EF.Functions.Like(u.FirstName, $"%{name}%") || EF.Functions.Like(u.MiddleName, $"%{name}%"));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.Email.Equals(email));

            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<User>> Users(int pageNumber, int pageSize, string name, string email, Guid roleId)
        {
            var query = _context.Users.Where(x => x.RoleId == roleId).Include(x => x.Role).AsQueryable();
            if (!string.IsNullOrEmpty(name))
                query = query
                    .Where(u => EF.Functions.Like(u.LastName, $"%{name}%") || EF.Functions.Like(u.FirstName, $"%{name}%") || EF.Functions.Like(u.MiddleName, $"%{name}%"));
            if (!string.IsNullOrEmpty(email))
                query = query.Where(u => u.Email.Equals(email));
            return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> Count() => await _context.Users.CountAsync();

        public async Task<int> Count(string roleName)
        {
            int count = 0;
            var role = await _roleService.Get(roleName);
            if (role != null)
                count = await _context.Users.Where(u => u.RoleId == role.Id).CountAsync();
            return count;
        }

        public async Task<int> Count(Guid roleId) => await _context.Users.Where(u => u.RoleId == roleId).CountAsync();

        public async Task<User> Status(string customerId, bool status)
        {
            var user = await _context.Users
                .Where(u => u.CustomerId.Equals(customerId))
                .FirstOrDefaultAsync();

            if (user != null)
                user.IsActive = status;
            return user;
        }

        public async Task<List<Role>> Roles() => await _roleService.Get();

        public async Task<Role> Role(Guid roleId) => await _roleService.Get(roleId);
    }
}