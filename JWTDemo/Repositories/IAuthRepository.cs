using JWTDemo.Models;
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
}
