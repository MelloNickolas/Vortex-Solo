using Microsoft.EntityFrameworkCore;
using Projeto.Domain.DTOs;
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

    public async Task<ResumoVendasResponse?> BuscarResumoAsync()
    {
        return await _context.Database
            .SqlQuery<ResumoVendasResponse>($"SELECT * FROM vw_ResumoVendas")
            .FirstOrDefaultAsync();
    }

    public async Task<List<ProdutoAbaixoMinimoResponse>> ListarProdutosAbaixoMinimoAsync()
    {
        return await _context.Database
            .SqlQuery<ProdutoAbaixoMinimoResponse>($"SELECT * FROM vw_ProdutosAbaixoMinimo ORDER BY Deficit DESC")
            .ToListAsync();
    }

    public async Task<List<VendaPorFormaPagamentoResponse>> ListarVendasPorFormaPagamentoAsync()
    {
        return await _context.Database
            .SqlQuery<VendaPorFormaPagamentoResponse>($"SELECT * FROM vw_VendasPorFormaPagamento ORDER BY Total DESC")
            .ToListAsync();
    }

    public async Task<List<ProdutoMaisVendidoResponse>> ListarProdutosMaisVendidosAsync(int top)
    {
        return await _context.Database
            .SqlQuery<ProdutoMaisVendidoResponse>($"EXEC sp_ProdutosMaisVendidos @Top = {top}")
            .ToListAsync();
    }
}
