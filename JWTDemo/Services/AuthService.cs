using JWTDemo.Models;
using JWTDemo.Repositories;
using JWTDemo.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTDemo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;

        public AuthService(IAuthRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        public async Task<string> GetToken(string userName)
        {
            var userViewModel = await GetUserViewModel(userName);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userViewModel.Id.ToString()),
                new Claim(ClaimTypes.Name, userViewModel.Name),

            };

            foreach (var role in userViewModel.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AuthConfig:Key").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private async Task<UserViewModel> GetUserViewModel(string userName)
        {
            var user = await _repository.Get(userName);

            if (user == null)
            {
                return null;
            }

            var roles = new List<string>();

            foreach (var userRole in user.UserRoles)
            {
                roles.Add(userRole.Role.Name);
            }

            return new UserViewModel
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Roles = roles.ToArray()
            };
        }

        public async Task<bool> Login(UserLoginViewModel viewModel)
        {

            // throw new NotImplementedException();
            var user = await _repository.Get(viewModel.Username);

            return VerifyPasswordHash(viewModel.Password, user.PasswordHash, user.PasswordSalt);

        }

        public async Task Register(UserRegisterViewModel viewModel)
        {
            //throw new NotImplementedException();
            var user = new User { Email = viewModel.Email, Name = viewModel.Name };

            byte[] passwordHash, passwordSalt;

            CreatePasswordHash(viewModel.Password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _repository.Create(user);

            foreach (var role in viewModel.Roles)
            {
                var modelRole = await _repository.GetRole(role);

                if (modelRole != null)
                {
                    var userRoleModel = new UserRole
                    {
                        User = user,
                        Role = modelRole,
                        UserId = user.Id,
                        RoleId = modelRole.Id
                    };

                    //  add user-roles
                    await _repository.Create(userRoleModel);
                }
            }

        }

        private void CreatePasswordHash(string rawPassword, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawPassword));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
