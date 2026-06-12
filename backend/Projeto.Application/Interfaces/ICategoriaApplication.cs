using Projeto.Application.DTOs.Categorias.Request;
using Projeto.Application.DTOs.Categorias.Response;

namespace Projeto.Application.Interfaces;

public interface ICategoriaApplication
{
    Task<List<CategoriaResponse>> ListarAsync();
    Task<CategoriaResponse> BuscarPorIdAsync(int id);
    Task<CategoriaResponse> CriarAsync(CategoriaRequest request);
    Task<CategoriaResponse> AtualizarAsync(int id, CategoriaRequest request);
    Task DeletarAsync(int id);
}
