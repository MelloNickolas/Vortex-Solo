using Projeto.Domain.Entities;

namespace Projeto.Repository.Interfaces;

public interface IClienteRepository
{
    Task<(IEnumerable<Cliente> Items, int Total)> GetPagedAsync(int page, int pageSize, string? busca, bool? ativo);
    Task<Cliente?> GetByIdAsync(int id);
    Task<Cliente?> GetByCpfAsync(string cpf);
    Task<Cliente> AddAsync(Cliente cliente);
    Task UpdateAsync(Cliente cliente);
    Task DeleteAsync(int id);
}
