using Projeto.Domain.Entities;

namespace Projeto.Repository.Interfaces;

public interface IProdutoRepository
{
    Task<(IEnumerable<Produto> Items, int Total)> GetPagedAsync(int page, int pageSize, string? nome, int? categoriaId);
    Task<Produto?> GetByIdAsync(int id);
    Task<IEnumerable<Produto>> GetAbaixoDoEstoqueMinimoAsync();
    Task<Produto> AddAsync(Produto produto);
    Task UpdateAsync(Produto produto);
    Task DeleteAsync(Produto produto);
}
