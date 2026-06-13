namespace Projeto.Application.DTOs.Auth.Response;

public class LoginResponse
{
    // Token JWT, o frontend guarda esse valor e manda em toda requisição autenticada
    public string Token { get; set; } = string.Empty;

    // Dados básicos do usuário logado, útil para exibir nome na tela sem chamar outra rota
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Quando o token expira, o frontend pode usar pra redirecionar pro login antes de expirar
    public DateTime Expiracao { get; set; }
}
