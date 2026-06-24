using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Enums;
using Projeto.Repository.Context;
using Projeto.Repository.FuncoesSQL;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    // Agrupa as vendas concluídas do mês/ano informado por dia
    // Retorna a quantidade de vendas e o total faturado em cada dia
    public async Task<List<VendaPorDiaResponse>> ListarVendasPorDiaAsync(int mes, int ano)
    {
        return await _context.Vendas
            .Where(v => v.DataVenda.Month == mes
                     && v.DataVenda.Year == ano
                     && v.Status == StatusVenda.Concluida)
            .GroupBy(v => v.DataVenda.Day)                 // agrupa por dia do mês
            .Select(g => new VendaPorDiaResponse
            {
                Dia = g.Key,
                Quantidade = g.Count(),
                Total = g.Sum(v => v.ValorTotal)
            })
            .OrderBy(r => r.Dia)
            .ToListAsync();
    }
}
