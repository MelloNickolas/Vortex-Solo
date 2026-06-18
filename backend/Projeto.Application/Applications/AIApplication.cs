using Projeto.Application.Interfaces;
using Projeto.Repository.Interfaces;
using Projeto.Services.Interfaces;

namespace Projeto.Application.Applications;

public class AIApplication : IAIApplication
{
    private readonly IAIService _aiService;
    private readonly IRelatorioRepository _relatorioRepository;

    public AIApplication(IAIService aiService, IRelatorioRepository relatorioRepository)
    {
        _aiService = aiService;
        _relatorioRepository = relatorioRepository;
    }

    public async Task<string> AnalisarVendasAsync()
    {
        var resumo = await _relatorioRepository.BuscarResumoAsync();

        var prompt = $"""
            Você é um analista financeiro do sistema de gestão Vortex. Analise os dados abaixo e gere um parágrafo curto e direto com insights sobre o desempenho de vendas. Escreva em português.

            Dados de vendas:
            - Total de vendas registradas: {resumo?.TotalVendas ?? 0}
            - Vendas concluídas: {resumo?.VendasConcluidas ?? 0}
            - Vendas canceladas: {resumo?.VendasCanceladas ?? 0}
            - Valor total faturado: R$ {resumo?.ValorTotalGeral ?? 0:F2}
            - Ticket médio por venda: R$ {resumo?.TicketMedio ?? 0:F2}

            Gere uma análise objetiva em 3 a 5 frases.
            """;

        return await _aiService.GetAiResponseAsync(prompt);
    }

    public async Task<string> AnalisarEstoqueAsync()
    {
        var produtos = await _relatorioRepository.ListarProdutosAbaixoMinimoAsync();

        if (produtos.Count == 0)
            return await _aiService.GetAiResponseAsync(
                "O estoque está saudável. Nenhum produto está abaixo do mínimo. Confirme isso ao usuário em uma frase positiva em português.");

        var listaProdutos = string.Join("\n", produtos.Select(p =>
            $"- {p.Nome} (categoria: {p.Categoria}): estoque atual = {p.EstoqueAtual}, mínimo = {p.EstoqueMinimo}, déficit = {p.Deficit}"));

        var prompt = $"""
            Você é um gestor de estoque do sistema Vortex. Os produtos abaixo estão com estoque abaixo do mínimo definido. Analise e gere recomendações práticas de reposição em português, de forma direta e objetiva.

            Produtos com estoque crítico:
            {listaProdutos}

            Gere recomendações em até 5 frases, priorizando os produtos com maior déficit.
            """;

        return await _aiService.GetAiResponseAsync(prompt);
    }
}
