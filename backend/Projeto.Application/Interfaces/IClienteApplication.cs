using Projeto.Application.Common;
using Projeto.Application.DTOs.Clientes.Request;
using Projeto.Application.DTOs.Clientes.Response;

namespace Projeto.Application.Interfaces;

public interface IClienteApplication
{
    Task<PagedResponse<ClienteResponse>> ListarPagedAsync(int page, int pageSize, string? busca, bool? ativo);
    Task<ClienteResponse> BuscarPorIdAsync(int id);
    Task<ClienteResponse> CriarAsync(ClienteRequest request);
    Task<ClienteResponse> AtualizarAsync(int id, ClienteRequest request);
    Task DeletarAsync(int id);
}
