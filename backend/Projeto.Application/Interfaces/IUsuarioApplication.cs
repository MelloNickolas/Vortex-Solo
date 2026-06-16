using Projeto.Application.Common;
using Projeto.Application.DTOs.Usuarios.Request;
using Projeto.Application.DTOs.Usuarios.Response;

namespace Projeto.Application.Interfaces;

public interface IUsuarioApplication
{
    Task<PagedResponse<UsuarioResponse>> ListarAsync(int page, int pageSize, string? busca, bool? ativo);
    Task<UsuarioResponse> BuscarPorIdAsync(int id);
    Task<UsuarioResponse> CriarAsync(UsuarioRequest request);
    Task<UsuarioResponse> AtualizarAsync(int id, UsuarioRequest request);
    Task AlterarSenhaAsync(int id, AlterarSenhaRequest request);
    Task DeletarAsync(int id);
    Task<UsuarioResponse> ReativarAsync(int id);
}
