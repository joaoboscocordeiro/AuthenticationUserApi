using AuthenticationUserApi.Dtos.Login;
using AuthenticationUserApi.Dtos.Register;
using AuthenticationUserApi.Dtos.Usuario;
using AuthenticationUserApi.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationUserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authInterface;

        public AuthController(IAuthInterface authInterface)
        {
            _authInterface = authInterface;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authInterface.Register(registerDto);

            if(!result.Status) return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authInterface.Login(loginDto);

            if (!result.Status) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("confirmar-email")]
        public async Task<IActionResult> ConfirmarEmail(string userId, string token)
        {
            var result = await _authInterface.ConfirmarEmail(userId, token);

            if (!result.Status) return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha(EsqueciSenhaDto esqueciSenhaDto)
        {
            var result = await _authInterface.EsqueciSenha(esqueciSenhaDto);

            if (!result.Status) return BadRequest(result);

            return Ok(result);
        }
    }
}
