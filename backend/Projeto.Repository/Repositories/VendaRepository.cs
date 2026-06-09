using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Domain.Enums;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class VendaRepository : BaseRepository, IVendaRepository
{
    public VendaRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Venda> Items, int Total)> GetPagedAsync(
        int page, int pageSize, StatusVenda? status, DateTime? de, DateTime? ate)
    {
        try
        {
            var query = _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(v => v.Status == status.Value);

            if (de.HasValue)
                query = query.Where(v => v.DataVenda >= de.Value);

            if (ate.HasValue)
                query = query.Where(v => v.DataVenda <= ate.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(v => v.DataVenda)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar vendas: {ex.Message}");
        }
    }

    public async Task<Venda?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.ID == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar venda {id}: {ex.Message}");
        }
    }

    public async Task<Venda> AddAsync(Venda venda)
    {
        try
        {
            await _context.Vendas.AddAsync(venda);
            await _context.SaveChangesAsync();
            return venda;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao registrar venda: {ex.Message}");
        }
    }

    // Utilizado para cancelamento — a lógica de reversão de estoque fica na camada Application
    public async Task UpdateAsync(Venda venda)
    {
        try
        {
            _context.Vendas.Update(venda);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar venda: {ex.Message}");
        }
    }
}
