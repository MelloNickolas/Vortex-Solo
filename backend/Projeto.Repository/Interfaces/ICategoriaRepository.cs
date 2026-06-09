using Projeto.Domain.Entities;

namespace Projeto.Repository.Interfaces;

public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> GetAllAsync();
    Task<Categoria?> GetByIdAsync(int id);
    Task<IEnumerable<Categoria>> SearchByNameAsync(string nome);
    Task<Categoria> AddAsync(Categoria categoria);
    Task UpdateAsync(Categoria categoria);
    Task DeleteAsync(int id);
}
