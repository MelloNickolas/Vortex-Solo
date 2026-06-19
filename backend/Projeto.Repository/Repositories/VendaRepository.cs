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
        int page, int pageSize, StatusVenda? status)
    {
        var query = _context.Vendas
            .Include(v => v.Cliente) // carrega o cliente
            .Include(v => v.Usuario) // carrega que registra a venda
            .Include(v => v.Itens) // carrega os itens da venda
                .ThenInclude(i => i.Produto) // carrega o produto da ItensVenda
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(v => v.Status == status.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(v => v.DataVenda)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        return (items, total);
    }


    public async Task<Venda?> GetByIdAsync(int id)
    {
        return await _context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Usuario)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(v => v.ID == id);
    }

    public async Task<Venda> AddAsync(Venda venda)
    {
        await _context.Vendas.AddAsync(venda); 
        await _context.SaveChangesAsync();     
        return venda;                         
    }

    public async Task UpdateAsync(Venda venda)
    {
        _context.Vendas.Update(venda);     
        await _context.SaveChangesAsync(); 
    }
}
