using Projeto.Application.Common;
using Projeto.Application.DTOs.Movimentacoes.Request;
using Projeto.Application.DTOs.Movimentacoes.Response;

namespace Projeto.Application.Interfaces;

public interface IMovimentacaoApplication
{
    Task<PagedResponse<MovimentacaoResponse>> ListarPagedAsync(
        int page, int pageSize, int? produtoId, string? tipo, DateTime? de, DateTime? ate);

    Task<MovimentacaoResponse> CriarAsync(MovimentacaoRequest request);
}
