using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class UsuarioRepository : BaseRepository, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Usuario> Items, int Total)> GetPagedAsync(
        int page, int pageSize, string? busca, bool? ativo)
    {
        var query = _context.Usuarios.AsQueryable();

        // Filtra por nome OU email (os dois campos que fazem sentido buscar em usuário)
        if (!string.IsNullOrWhiteSpace(busca))
            query = query.Where(u =>
                u.Nome.ToLower().Contains(busca.ToLower()) ||
                u.Email.ToLower().Contains(busca.ToLower()));

        // Filtra por status ativo/inativo se enviado
        if (ativo.HasValue)
            query = query.Where(u => u.Ativo == ativo.Value);

        // Conta o total com os filtros aplicados (antes de paginar)
        var total = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.ID == id);
    }

    // Utilizado pelo serviço de autenticação para validar o login
    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }

    // Soft delete — Application busca o objeto, valida e passa aqui já resolvido
    public async Task DeleteAsync(Usuario usuario)
    {
        usuario.Ativo = false;
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }
}
