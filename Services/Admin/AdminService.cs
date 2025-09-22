using AuthenticationUserApi.Dtos.Usuario;
using AuthenticationUserApi.Models;
using Microsoft.AspNetCore.Identity;
using WebApiUser.Models;

namespace AuthenticationUserApi.Services.Admin
{
    public class AdminService : IAdminInterface
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseModel<List<ListagemUserRoleDto>>> GetUsuariosComRoles()
        {
            ResponseModel<List<ListagemUserRoleDto>> response = new ResponseModel<List<ListagemUserRoleDto>>();

            try
            {
                var users = _userManager.Users.ToList();

                var usuariosComRoles = new List<ListagemUserRoleDto>();

                foreach(var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    usuariosComRoles.Add(new ListagemUserRoleDto
                    {
                        Id = user.Id,
                        Usuario = user.UserName,
                        Email = user.Email,
                        Roles = roles.ToList()
                    });
                }

                response.Mensagem = "Usuários recuperados!";
                response.Dados = usuariosComRoles;
                return response;
            }
            catch(Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }
    }
}
