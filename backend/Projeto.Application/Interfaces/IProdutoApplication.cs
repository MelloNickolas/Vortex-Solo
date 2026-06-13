using Projeto.Application.Common;
using Projeto.Application.DTOs.Produtos.Request;
using Projeto.Application.DTOs.Produtos.Response;

namespace Projeto.Application.Interfaces;

public interface IProdutoApplication
{
    Task<PagedResponse<ProdutoResponse>> ListarPagedAsync(int page, int pageSize, string? nome, int? categoriaId);
    Task<ProdutoResponse> BuscarPorIdAsync(int id);
    Task<ProdutoResponse> CriarAsync(ProdutoRequest request);
    Task<ProdutoResponse> AtualizarAsync(int id, ProdutoRequest request);
    Task DeletarAsync(int id);
}
