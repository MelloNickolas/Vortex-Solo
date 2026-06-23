namespace Projeto.Application.Interfaces;

public interface IAIApplication
{
    // Cada método retorna os dados do banco + a análise gerada pela IA
    Task<(object Dados, string Resposta)> AnalisarVendasAsync();
    Task<(object Dados, string Resposta)> AnalisarEstoqueAsync();
    Task<(object Dados, string Resposta)> AnalisarFormasPagamentoAsync();
    Task<(object Dados, string Resposta)> AnalisarProdutosMaisVendidosAsync();
    Task<(object Dados, string Resposta)> AnalisarClienteAsync(int clienteId);
}
