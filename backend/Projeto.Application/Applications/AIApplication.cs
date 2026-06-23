using Projeto.Application.Interfaces;
using Projeto.Repository.Interfaces;
using Projeto.Services.Interfaces;

namespace Projeto.Application.Applications;

// Busca dados reais do banco, monta o prompt e envia para o AIService
// Cada método retorna uma tupla: os dados (para a tabela) + a resposta da IA (texto)
public class AIApplication : IAIApplication
{
    private readonly IAIService _aiService;
    private readonly IRelatorioRepository _relatorioRepository;

    public AIApplication(IAIService aiService, IRelatorioRepository relatorioRepository)
    {
        _aiService = aiService;
        _relatorioRepository = relatorioRepository;
    }

    // VIEW: vw_ResumoVendas — analisa o desempenho financeiro geral
    public async Task<(object Dados, string Resposta)> AnalisarVendasAsync()
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

        var resposta = await _aiService.GetAiResponseAsync(prompt);
        return (resumo!, resposta);
    }

    // VIEW: vw_ProdutosAbaixoMinimo — recomendações de reposição de estoque
    public async Task<(object Dados, string Resposta)> AnalisarEstoqueAsync()
    {
        var produtos = await _relatorioRepository.ListarProdutosAbaixoMinimoAsync();

        if (produtos.Count == 0)
        {
            var msg = await _aiService.GetAiResponseAsync(
                "O estoque está saudável. Nenhum produto está abaixo do mínimo. Confirme isso ao usuário em uma frase positiva em português.");
            return (produtos, msg);
        }

        var listaProdutos = string.Join("\n", produtos.Select(p =>
            $"- {p.Nome} (categoria: {p.Categoria}): estoque atual = {p.EstoqueAtual}, mínimo = {p.EstoqueMinimo}, déficit = {p.Deficit}"));

        var prompt = $"""
            Você é um gestor de estoque do sistema Vortex. Os produtos abaixo estão com estoque abaixo do mínimo definido. Analise e gere recomendações práticas de reposição em português, de forma direta e objetiva.

            Produtos com estoque crítico:
            {listaProdutos}

            Gere recomendações em até 5 frases, priorizando os produtos com maior déficit.
            """;

        var resposta = await _aiService.GetAiResponseAsync(prompt);
        return (produtos, resposta);
    }

    // VIEW: vw_VendasPorFormaPagamento — analisa preferências de pagamento dos clientes
    public async Task<(object Dados, string Resposta)> AnalisarFormasPagamentoAsync()
    {
        var formas = await _relatorioRepository.ListarVendasPorFormaPagamentoAsync();

        if (formas.Count == 0)
        {
            var msg = await _aiService.GetAiResponseAsync(
                "Não há dados de formas de pagamento. Informe isso ao usuário em uma frase em português.");
            return (formas, msg);
        }

        var lista = string.Join("\n", formas.Select(f =>
            $"- {f.FormaPagamentoNome}: {f.Quantidade} venda(s), total de R$ {f.Total:F2}"));

        var prompt = $"""
            Você é um analista de vendas do sistema Vortex. Analise as formas de pagamento utilizadas pelos clientes e gere insights sobre o comportamento de pagamento. Escreva em português.

            Formas de pagamento:
            {lista}

            Gere uma análise em 3 a 5 frases destacando a forma mais usada, o total movimentado e alguma recomendação prática.
            """;

        var resposta = await _aiService.GetAiResponseAsync(prompt);
        return (formas, resposta);
    }

    // SP: sp_ProdutosMaisVendidos — analisa os produtos com melhor desempenho
    public async Task<(object Dados, string Resposta)> AnalisarProdutosMaisVendidosAsync()
    {
        var produtos = await _relatorioRepository.ListarProdutosMaisVendidosAsync(10);

        if (produtos.Count == 0)
        {
            var msg = await _aiService.GetAiResponseAsync(
                "Não há dados de produtos vendidos. Informe isso ao usuário em uma frase em português.");
            return (produtos, msg);
        }

        var lista = string.Join("\n", produtos.Select((p, i) =>
            $"{i + 1}. {p.Produto} (categoria: {p.Categoria}): {p.TotalVendido} unidades, R$ {p.TotalFaturado:F2}"));

        var prompt = $"""
            Você é um analista comercial do sistema Vortex. Analise o ranking dos produtos mais vendidos abaixo e gere insights sobre o desempenho do mix de produtos. Escreva em português.

            Top 10 produtos mais vendidos:
            {lista}

            Gere uma análise em 3 a 5 frases destacando os destaques, padrões de categoria e alguma recomendação estratégica.
            """;

        var resposta = await _aiService.GetAiResponseAsync(prompt);
        return (produtos, resposta);
    }

    // FUNCTION: fn_TotalVendasCliente — analisa o histórico de compras de um cliente específico
    public async Task<(object Dados, string Resposta)> AnalisarClienteAsync(int clienteId)
    {
        var total = await _relatorioRepository.ConsultarTotalClienteAsync(clienteId);

        var prompt = $"""
            Você é um analista de relacionamento com clientes do sistema Vortex. Com base no total de compras do cliente abaixo, gere um breve comentário sobre o perfil desse cliente. Escreva em português.

            Total faturado com esse cliente: R$ {total:F2}

            Gere uma análise em 2 a 3 frases classificando o perfil do cliente e sugerindo alguma ação comercial.
            """;

        var resposta = await _aiService.GetAiResponseAsync(prompt);
        return (new { total }, resposta);
    }
}
