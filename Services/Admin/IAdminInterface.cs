using AuthenticationUserApi.Dtos.Usuario;
using WebApiUser.Models;

namespace AuthenticationUserApi.Services.Admin
{
    public interface IAdminInterface
    {
        Task<ResponseModel<List<ListagemUserRoleDto>>> GetUsuariosComRoles();
    }
}
