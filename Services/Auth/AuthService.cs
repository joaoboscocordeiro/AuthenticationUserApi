using AuthenticationUserApi.Dtos;
using AuthenticationUserApi.Models;
using Microsoft.AspNetCore.Identity;
using WebApiUser.Models;

namespace AuthenticationUserApi.Services.Auth
{
    public class AuthService : IAuthInterface
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseModel<string>> Register(RegisterDto registerDto)
        {
            ResponseModel<string> response = new ResponseModel<string>();

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
