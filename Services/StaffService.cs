using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Data;
using Identity.Data.Dtos.Request.Auth;
using Identity.Data.Dtos.Request.User;
using Identity.Data.Dtos.Response.User;
using Identity.Data.Models;
using Identity.Helpers;
using Identity.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Identity.Services
{
    public class StaffService : IStaffService
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        private readonly DataContext _context;

        public StaffService(INotificationService notificationService, IMapper mapper, IUserService userService, DataContext context)
        {
            _notificationService = notificationService;
            _mapper = mapper;
            _userService = userService;
            _context = context;
        }

        public async Task<Role> AssignPermmission(string roleId, List<Guid> permissions)
        {
            var role = await _context.Roles.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == new Guid(roleId));
            var foundPermissions = await _context.Permissions
                .Where(permission => permissions.Any(p2 => p2 == permission.Id))
                .ToListAsync();
            if (foundPermissions.Count > 0)
            {
                if (role.Permissions == null)
                    role.Permissions = foundPermissions;
                else
                {
                    List<Permission> currentPermission = role.Permissions.ToList();
                    currentPermission.AddRange(foundPermissions);
                    role.Permissions = currentPermission.Distinct().ToList();
                }

                _context.Update(role);
                await _context.SaveChangesAsync();
            }
            return role;
        }

        public async Task<Staff> CreateStaff(CreateStaff createStaff)
        {
            var user = _mapper.Map<User>(createStaff);
            string password = PasswordHelper.GeneratePassword();
            user.Password = BCrypt.Net.BCrypt.HashPassword(password);
            var createResponse = await _userService.CreateStaff(user, createStaff.RoleId);
            _notificationService.SendStaffAccountEmail(user.Email, user.FirstName + " " + user.LastName);
            return _mapper.Map<Staff>(createResponse.Item1);
        }

        public async Task<bool> Exist(string email) => await _userService.Exist(email);

        public async Task<Tuple<List<Staff>, int>> Get(int pageNumber, int pageSize, StaffFilter staffFilter)
        {
            var users = await _userService.Users(pageNumber, pageSize, staffFilter.Name, staffFilter.Email, staffFilter.Position);
            var totalRecords = await _userService.Count(new Guid(staffFilter.Position));
            var staffs = _mapper.Map<List<Staff>>(users);
            return new Tuple<List<Staff>, int>(staffs, totalRecords);
        }

        public async Task<List<Role>> Role() => await _userService.Roles();
        public async Task<List<Permission>> Pemissions() => await _context.Permissions.ToListAsync();

        public async Task<Role> Role(string roleId) => await _userService.Role(new Guid(roleId));
    }
}