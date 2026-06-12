namespace Projeto.Application.Exceptions;

// Lançada quando o usuário não tem permissão para executar uma ação
// Exemplo: tentar trocar a senha de outro usuário, ou acessar sem estar autenticado
// O middleware converte isso em HTTP 401 Unauthorized
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string mensagem) : base(mensagem) { }
}
