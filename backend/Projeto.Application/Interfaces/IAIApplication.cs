namespace Projeto.Application.Interfaces;

public interface IAIApplication
{
    Task<string> AnalisarVendasAsync();
    Task<string> AnalisarEstoqueAsync();
}
