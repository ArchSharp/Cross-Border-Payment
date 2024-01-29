using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Data.Models;
using Identity.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Identity.Services
{
    public class RoleService : IRoleService
    {
        private readonly DataContext _context;
        public RoleService(DataContext context)
        {
            _context = context;
        }

        public async Task<Role> Get(string name) => await _context.Roles.FirstOrDefaultAsync(r => r.Name.Equals(name));

        public async Task<Role> Get(Guid roleId) => await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

        public async Task<List<Role>> Get() => await _context.Roles
            .Where(r => r.Name != "Admin")
            .ToListAsync();
    }
}