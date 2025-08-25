using AuthenticationUserApi.Dtos;
using WebApiUser.Models;

namespace AuthenticationUserApi.Services.Auth
{
    public interface IAuthInterface
    {
        Task<ResponseModel<string>> Register(RegisterDto registerDto);
    }
}
