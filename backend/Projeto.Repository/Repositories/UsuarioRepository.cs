using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class UsuarioRepository : BaseRepository, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios
            .OrderBy(u => u.Nome)
            .ToListAsync();
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
