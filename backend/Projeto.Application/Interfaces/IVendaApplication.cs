using Projeto.Application.Common;
using Projeto.Application.DTOs.Vendas.Request;
using Projeto.Application.DTOs.Vendas.Response;

namespace Projeto.Application.Interfaces;

public interface IVendaApplication
{
    Task<PagedResponse<VendaResponse>> ListarPagedAsync(int page, int pageSize, string? status, DateTime? de, DateTime? ate);
    Task<VendaResponse> BuscarPorIdAsync(int id);
    Task<VendaResponse> CriarAsync(VendaRequest request);
    Task<VendaResponse> CancelarAsync(int id, int usuarioId);
}
