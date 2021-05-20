using JWTDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTDemo.Services
{
   public interface IAuthService
    {
        Task<bool> Login(UserLoginViewModel viewModel);
        Task Register(UserRegisterViewModel viewModel);
        Task<string> GetToken(string userName);

    }
}
