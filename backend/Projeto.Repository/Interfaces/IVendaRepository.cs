using Projeto.Domain.Entities;
using Projeto.Domain.Enums;

namespace Projeto.Repository.Interfaces;

public interface IVendaRepository
{
    Task<(IEnumerable<Venda> Items, int Total)> GetPagedAsync(int page, int pageSize, StatusVenda? status, DateTime? de, DateTime? ate);
    Task<Venda?> GetByIdAsync(int id);
    Task<Venda> AddAsync(Venda venda);
    Task UpdateAsync(Venda venda);
}
