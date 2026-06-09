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
        try
        {
            var query = _context.Clientes
                .Include(c => c.Cidade)
                    .ThenInclude(ci => ci.Estado)
                .AsQueryable();

            // Filtro unificado: busca por nome OU CPF
            if (!string.IsNullOrWhiteSpace(busca))
                query = query.Where(c =>
                    c.Nome.ToLower().Contains(busca.ToLower()) ||
                    c.CPF.Contains(busca));

            if (ativo.HasValue)
                query = query.Where(c => c.Ativo == ativo.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar clientes: {ex.Message}");
        }
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Clientes
                .Include(c => c.Cidade)
                    .ThenInclude(ci => ci.Estado)
                .FirstOrDefaultAsync(c => c.ID == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar cliente {id}: {ex.Message}");
        }
    }

    public async Task<Cliente?> GetByCpfAsync(string cpf)
    {
        try
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.CPF == cpf);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar cliente por CPF: {ex.Message}");
        }
    }

    public async Task<Cliente> AddAsync(Cliente cliente)
    {
        try
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar cliente: {ex.Message}");
        }
    }

    public async Task UpdateAsync(Cliente cliente)
    {
        try
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar cliente: {ex.Message}");
        }
    }

    // Soft delete — mantém histórico de vendas do cliente intacto
    public async Task DeleteAsync(int id)
    {
        try
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ID == id)
                ?? throw new KeyNotFoundException($"Cliente {id} não encontrado.");

            cliente.Ativo = false;
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao desativar cliente: {ex.Message}");
        }
    }
}
