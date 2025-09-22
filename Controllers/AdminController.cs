using AuthenticationUserApi.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationUserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminInterface _adminInterface;

        public AdminController(IAdminInterface adminInterface)
        {
            _adminInterface = adminInterface;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuariosComRoles()
        {
            var resultado = await _adminInterface.GetUsuariosComRoles();

            if (!resultado.Status) return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}
