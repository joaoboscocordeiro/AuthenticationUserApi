﻿namespace AuthenticationUserApi.Dtos.Usuario
{
    public class ResetarSenhaDto
    {
        public string Usuario { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
    }
}
