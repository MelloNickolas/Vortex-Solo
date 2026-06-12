using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class ClienteRepository : BaseRepository, IClienteRepository
{
    public ClienteRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Cliente> Items, int Total)> GetPagedAsync(
        int page, int pageSize, string? busca, bool? ativo)
    {
        var query = _context.Clientes
            /* aqui colocamos include, ele serve como um JOIN no SQL, ele vai carregar tudo junto, em vez de fazer 2 viagens*/
            .Include(c => c.Cidade)
                /* Aqui é um Include encadeado */
                .ThenInclude(ci => ci.Estado)
            .AsQueryable(); // ele basicamente ffaza voce aceitar WHERE e ORDERBY depois de buscar.

        // Filtro unificado, busca por nome OU CPF
        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(c =>
                c.Nome.ToLower().Contains(busca.ToLower()) ||
                c.CPF.Contains(busca));

        if (ativo.HasValue)
            query = query.Where(c => c.Ativo == ativo.Value);


        // ele vai contar depois de tudo quantos produtos tem ao todo, ai o nosso front-end só usa esse numero .
        var total = await query.CountAsync();

        var items = await query
            // primeiro ordenamos cada produto por ordem alfabetica.
            .OrderBy(c => c.Nome)
            /* A fórmula (page - 1) * pageSize calcula exatamente quantos pular para chegar na página certa. 
            vamos supor que a pageSize é 10, e o front quer a page 2, o cálculo vai ficar assim:
            (2 - 1) * 10 = 10 ----- ou seja, ele vai pular os 10 primeiros registros. */
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _context.Clientes
            .Include(c => c.Cidade)
                .ThenInclude(ci => ci.Estado)
            .FirstOrDefaultAsync(c => c.ID == id);
    }

    public async Task<Cliente?> GetByCpfAsync(string cpf)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.CPF == cpf);
    }

    public async Task<Cliente> AddAsync(Cliente cliente)
    {
        await _context.Clientes.AddAsync(cliente);
        await _context.SaveChangesAsync();
        return cliente;
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Cliente cliente)
    {
        cliente.Ativo = false;
        _context.Clientes.Update(cliente);
        await _context.SaveChangesAsync();
    }
}
