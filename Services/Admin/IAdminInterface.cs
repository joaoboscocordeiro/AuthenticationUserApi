using AuthenticationUserApi.Dtos.Usuario;
using WebApiUser.Models;

namespace AuthenticationUserApi.Services.Admin
{
    public interface IAdminInterface
    {
        Task<ResponseModel<List<ListagemUserRoleDto>>> GetUsuariosComRoles();
        Task<ResponseModel<string>> AdicionarRoles(AtualizarUserRoleDto atualizarUserRoleDto);
        Task<ResponseModel<string>> RemoverRoles(AtualizarUserRoleDto atualizarUserRoleDto);
    }
}
