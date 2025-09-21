using AuthenticationUserApi.Dtos.Login;
using AuthenticationUserApi.Dtos.Register;
using AuthenticationUserApi.Dtos.Usuario;
using WebApiUser.Models;

namespace AuthenticationUserApi.Services.Auth
{
    public interface IAuthInterface
    {
        Task<ResponseModel<string>> Register(RegisterDto registerDto);
        Task<ResponseModel<string>> Login(LoginDto loginDto);
        Task<ResponseModel<string>> ConfirmarEmail(string userId, string token);
        Task<ResponseModel<string>> EsqueciSenha(EsqueciSenhaDto esqueciSenhaDto);
        Task<ResponseModel<string>> ResetarSenha(ResetarSenhaDto resetarSenhaDto);
    }
}
