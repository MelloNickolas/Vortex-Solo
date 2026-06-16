using Projeto.Domain.Entities;

namespace Projeto.Repository.Interfaces;

public interface IUsuarioRepository
{
    // Retorna uma página de usuários com filtros de busca e status
    Task<(IEnumerable<Usuario> Items, int Total)> GetPagedAsync(int page, int pageSize, string? busca, bool? ativo);
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario> AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
    Task DeleteAsync(Usuario usuario);
}
