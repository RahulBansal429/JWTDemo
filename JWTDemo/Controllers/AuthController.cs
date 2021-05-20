using JWTDemo.Services;
using JWTDemo.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel viewModel)
        {
            var result = await _service.Login(viewModel);
            if (result)
            {
                return Ok(new { Token = await _service.GetToken(viewModel.Username) });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterViewModel viewModel)
        {
            await _service.Register(viewModel);
            return Ok();
        }
    }
}
