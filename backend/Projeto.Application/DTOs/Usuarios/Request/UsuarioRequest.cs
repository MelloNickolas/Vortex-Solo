namespace Projeto.Application.DTOs.Usuarios.Request;

// Usado para criar e atualizar perfil do usuário
public class UsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
