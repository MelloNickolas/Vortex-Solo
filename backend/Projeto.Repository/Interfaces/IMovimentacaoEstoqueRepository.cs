using Projeto.Domain.Entities;
using Projeto.Domain.Enums;

namespace Projeto.Repository.Interfaces;

public interface IMovimentacaoEstoqueRepository
{
    /// Retorna movimentações paginadas com filtros opcionais de produto, tipo e período.
    Task<(IEnumerable<MovimentacaoEstoque> Items, int Total)> GetPagedAsync(
        int page, int pageSize, int? produtoId, TipoMovimentacao? tipo, DateTime? de, DateTime? ate);

    Task<MovimentacaoEstoque> AddAsync(MovimentacaoEstoque movimentacao);
}
