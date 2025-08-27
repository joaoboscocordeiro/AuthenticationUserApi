using AuthenticationUserApi.Dtos.Login;
using AuthenticationUserApi.Dtos.Register;
using AuthenticationUserApi.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApiUser.Models;

namespace AuthenticationUserApi.Services.Auth
{
    public class AuthService : IAuthInterface
    {
        ResponseModel<string> response = new ResponseModel<string>();

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseModel<string>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDto.Usuario);

                if (user == null)
                {
                    response.Mensagem = "Credenciais inválidas!";
                    response.Status = false;
                    return response;
                }

                var validPassword = await _userManager.CheckPasswordAsync(user, loginDto.Senha);

                if (!validPassword)
                {
                    response.Mensagem = "Credenciais inválidas!";
                    response.Status = false;
                    return response;
                }

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("nome", user.NomeCompleto),
                    new Claim("usuario", user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                var userRoles = await _userManager.GetRolesAsync(user);

                foreach(var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<string>> Register(RegisterDto registerDto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    Email = registerDto.Email,
                    NomeCompleto = registerDto.NomeCompleto,
                    UserName = registerDto.Usuario
                };

                var result = await _userManager.CreateAsync(user, registerDto.Senha);

                if (!result.Succeeded)
                {
                    response.Dados = string.Join(", ", result.Errors.Select(e => e.Description));
                    response.Status = false;
                    return response;
                }

                var rolesInvalidas = new List<string>();

                foreach (var role in registerDto.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        rolesInvalidas.Add(role);
                    }
                }

                if (rolesInvalidas.Any())
                {
                    response.Dados = $"As seguintes roles não existem: {string.Join(", ", rolesInvalidas)}";
                    response.Status = false;
                    return response;
                }

                foreach(var role in registerDto.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                response.Mensagem = "Usuário cadastrado com sucesso!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }
    }
}
