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
        try
        {
            return await _context.Usuarios
                .OrderBy(u => u.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar usuários: {ex.Message}");
        }
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.ID == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar usuário {id}: {ex.Message}");
        }
    }

    // Utilizado pelo serviço de autenticação para validar o login
    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        try
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar usuário por e-mail: {ex.Message}");
        }
    }

    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        try
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar usuário: {ex.Message}");
        }
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        try
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar usuário: {ex.Message}");
        }
    }

    // Soft delete — apenas desativa o usuário, mantendo histórico de vendas e movimentações
    public async Task DeleteAsync(int id)
    {
        try
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.ID == id)
                ?? throw new KeyNotFoundException($"Usuário {id} não encontrado.");

            usuario.Ativo = false;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao desativar usuário: {ex.Message}");
        }
    }
}
