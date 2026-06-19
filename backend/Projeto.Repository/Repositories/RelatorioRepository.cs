using Microsoft.EntityFrameworkCore;
using Projeto.Repository.FuncoesSQL;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class RelatorioRepository : IRelatorioRepository
{
    private readonly AppDbContext _context;

    public RelatorioRepository(AppDbContext context)
    {
        _context = context;
    }

    // VIEW vw_ResumoVendas
    // Retorna totais gerais como a quantidade de vendas, valor faturado, ticket médio, concluídas e canceladas.
    public async Task<ResumoVendasResponse?> BuscarResumoAsync()
    {
        return await _context.Database
            .SqlQuery<ResumoVendasResponse>($"SELECT * FROM vw_ResumoVendas")
            .FirstOrDefaultAsync();
    }

    // vw_ProdutosAbaixoMinimo
    // Lista produtos onde o EstoqueAtual está abaixo do EstoqueMinimo definido no cadastro.
    public async Task<List<ProdutoAbaixoMinimoResponse>> ListarProdutosAbaixoMinimoAsync()
    {
        return await _context.Database
            .SqlQuery<ProdutoAbaixoMinimoResponse>($"SELECT * FROM vw_ProdutosAbaixoMinimo ORDER BY Deficit DESC")
            .ToListAsync();
    }

    // VIEW: vw_VendasPorFormaPagamento
    // Agrupa as vendas concluídas por forma de pagamento, retornando quantidade e total faturado em cada uma.
    public async Task<List<VendaPorFormaPagamentoResponse>> ListarVendasPorFormaPagamentoAsync()
    {
        return await _context.Database
            .SqlQuery<VendaPorFormaPagamentoResponse>($"SELECT * FROM vw_VendasPorFormaPagamento ORDER BY Total DESC")
            .ToListAsync();
    }

    // STORED PROCEDURE: sp_ProdutosMaisVendidos @Top
    // Retorna os N produtos mais vendidos (por quantidade) em vendas com status Concluída.
    public async Task<List<ProdutoMaisVendidoResponse>> ListarProdutosMaisVendidosAsync(int top)
    {
        return await _context.Database
            .SqlQuery<ProdutoMaisVendidoResponse>($"EXEC sp_ProdutosMaisVendidos @Top = {top}")
            .ToListAsync();
    }

    // FUNCTION: fn_TotalVendasCliente(@ClienteID)
    // Retorna o valor total faturado em vendas concluídas de um cliente específico.
    public async Task<decimal> ConsultarTotalClienteAsync(int clienteId)
    {
        var result = await _context.Database
            .SqlQuery<TotalClienteResponse>($"SELECT dbo.fn_TotalVendasCliente({clienteId}) AS Total")
            .FirstOrDefaultAsync();

        return result?.Total ?? 0;
    }
}
