using Projeto.Application.Interfaces;
using Projeto.Repository.Interfaces;
using Projeto.Services.Interfaces;

namespace Projeto.Application.Applications;


// O que ela faz busca dados reais do banco, monta o prompt contextualizado e envia para o AIService
public class AIApplication : IAIApplication
{
    private readonly IAIService _aiService;// faz a chamada http pro github Models
    private readonly IRelatorioRepository _relatorioRepository; //busca os dados pelos views

    public AIApplication(IAIService aiService, IRelatorioRepository relatorioRepository)
    {
        _aiService = aiService;
        _relatorioRepository = relatorioRepository;
    }

    // ve o financeiro de vendas usando dados da VIEW vw_ResumoVendas
    // ele busca os dados do banco, dps monta o prompt, envia pra ia, e dps retorna uma analisa
    public async Task<string> AnalisarVendasAsync()
    {
        // Busca os dados da VIEW vw_ResumoVendas 
        var resumo = await _relatorioRepository.BuscarResumoAsync();

        // Monta o prompt com os dados reais do banco interpolados
        // :F2 formata o decimal com 2 casas 
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

        // Envia o prompt para o AIService que faz a chamada HTTP para o GitHub Models
        return await _aiService.GetAiResponseAsync(prompt);
    }








    // Analisa o estoque crítico usando dados da VIEW vw_ProdutosAbaixoMinimo
    // aqui ele vai buscar produtos que estao em queda, monta a lista no prompt, envia pra IA e retorna algumas recomendações.
    public async Task<string> AnalisarEstoqueAsync()
    {
        // Busca todos os produtos cujo estoque atual está abaixo do mínimo definido
        var produtos = await _relatorioRepository.ListarProdutosAbaixoMinimoAsync();

        // Evita enviar um prompt com lista vazia para a IA se naoo tiver produtos abaixo do minimo
        if (produtos.Count == 0)
            return await _aiService.GetAiResponseAsync(
                "O estoque está saudável. Nenhum produto está abaixo do mínimo. Confirme isso ao usuário em uma frase positiva em português.");

        // string.Join une todas as linhas com \n ------- é uma quebra de linha
        // Cada linha vai mostrar o nome, categoria, estoque atual, mínimo e déficit
        var listaProdutos = string.Join("\n", produtos.Select(p =>
            $"- {p.Nome} (categoria: {p.Categoria}): estoque atual = {p.EstoqueAtual}, mínimo = {p.EstoqueMinimo}, déficit = {p.Deficit}"));

        // A instrução "priorizando os produtos com maior déficit" guia a IA a focar nos mais urgentes
        var prompt = $"""
            Você é um gestor de estoque do sistema Vortex. Os produtos abaixo estão com estoque abaixo do mínimo definido. Analise e gere recomendações práticas de reposição em português, de forma direta e objetiva.

            Produtos com estoque crítico:
            {listaProdutos}

            Gere recomendações em até 5 frases, priorizando os produtos com maior déficit.
            """;

        return await _aiService.GetAiResponseAsync(prompt);
    }
}
