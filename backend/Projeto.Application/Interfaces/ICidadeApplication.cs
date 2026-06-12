using Projeto.Application.DTOs.Cidades.Response;

namespace Projeto.Application.Interfaces;

// Cidade é somente leitura — inserida diretamente no banco via SQL
public interface ICidadeApplication
{
    Task<List<CidadeResponse>> ListarPorEstadoAsync(int estadoId);
    Task<CidadeResponse> BuscarPorIdAsync(int id);
}
