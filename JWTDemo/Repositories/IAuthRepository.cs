using JWTDemo.Data;
using JWTDemo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTDemo.Repositories
{
    public interface IAuthRepository
    {
        Task Create(User user);
        Task Create(Role role);
        Task<User> Get(string userName);
        Task Create(UserRole userRole);
        Task<Role> GetRole(string roleName);
        Task<IEnumerable<UserRole>> GetRoles(string userName);
    }


    public class AuthRepository : IAuthRepository
    {
        private readonly AuthContext _context;

        public AuthRepository(AuthContext context)
        {
            _context = context;
        }

        public async Task Create(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task Create(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task Create(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task<User> Get(string userName)
        {
            return await _context
                .Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == userName);
        }

        public async Task<Role> GetRole(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<IEnumerable<UserRole>> GetRoles(string userName)
        {
            return await _context
                .UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.User.Email == userName)
                .ToListAsync();
        }
    }
}
