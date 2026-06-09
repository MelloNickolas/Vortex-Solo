using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Domain.Enums;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class MovimentacaoEstoqueRepository : BaseRepository, IMovimentacaoEstoqueRepository
{
    public MovimentacaoEstoqueRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<MovimentacaoEstoque> Items, int Total)> GetPagedAsync(
        int page, int pageSize, int? produtoId, TipoMovimentacao? tipo, DateTime? de, DateTime? ate)
    {
        try
        {
            var query = _context.MovimentacoesEstoque
                .Include(m => m.Produto)
                .Include(m => m.Usuario)
                .AsQueryable();

            if (produtoId.HasValue)
                query = query.Where(m => m.ProdutoID == produtoId.Value);

            if (tipo.HasValue)
                query = query.Where(m => m.Tipo == tipo.Value);

            if (de.HasValue)
                query = query.Where(m => m.DataMovimento >= de.Value);

            if (ate.HasValue)
                query = query.Where(m => m.DataMovimento <= ate.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.DataMovimento)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar movimentações: {ex.Message}");
        }
    }

    public async Task<MovimentacaoEstoque> AddAsync(MovimentacaoEstoque movimentacao)
    {
        try
        {
            await _context.MovimentacoesEstoque.AddAsync(movimentacao);
            await _context.SaveChangesAsync();
            return movimentacao;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao registrar movimentação: {ex.Message}");
        }
    }
}
