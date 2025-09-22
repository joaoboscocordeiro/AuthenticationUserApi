namespace AuthenticationUserApi.Dtos.Usuario
{
    public class AtualizarUserRoleDto
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
