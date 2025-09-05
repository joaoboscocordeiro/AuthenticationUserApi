using AuthenticationUserApi.Dtos.Login;
using AuthenticationUserApi.Dtos.Register;
using AuthenticationUserApi.Models;
using AuthenticationUserApi.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiUser.Models;

namespace AuthenticationUserApi.Services.Auth
{
    public class AuthService : IAuthInterface
    {
        ResponseModel<string> response = new ResponseModel<string>();

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailInterface _emailInterface;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailInterface emailInterface)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailInterface = emailInterface;
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

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: authClaims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                    signingCredentials: creds
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                response.Dados = tokenString;
                response.Mensagem = "Usuário logado com sucesso!";
                return response;
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

                await confirmacaoEmail(user);

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

        private async Task confirmacaoEmail(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmUrl = $"https://localhost:7011/api/auth/confirmar-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            var mensagem = $"<h3>Confirme seu a-mail</h3><p>Clique no link para confirmar: <a href='{confirmUrl}'>Confirmar</a></p>";

            await _emailInterface.EnviarEmailAsync(user.Email, "Confirmação de E-mail", mensagem);
        }
    }
}
