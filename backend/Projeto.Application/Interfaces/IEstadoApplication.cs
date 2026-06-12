using Projeto.Application.DTOs.Estados.Response;

namespace Projeto.Application.Interfaces;

// Estado é somente leitura — inserido diretamente no banco via SQL
public interface IEstadoApplication
{
    Task<List<EstadoResponse>> ListarAsync();
    Task<EstadoResponse> BuscarPorIdAsync(int id);
}
