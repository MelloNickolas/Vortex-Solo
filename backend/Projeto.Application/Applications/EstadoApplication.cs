using Projeto.Application.DTOs.Estados.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class EstadoApplication : IEstadoApplication
{
    private readonly IEstadoRepository _estadoRepository;

    public EstadoApplication(IEstadoRepository estadoRepository)
    {
        _estadoRepository = estadoRepository;
    }

    public async Task<List<EstadoResponse>> ListarAsync()
    {
        try
        {
            var estados = await _estadoRepository.GetAllAsync();

            return estados.Select(e => new EstadoResponse
            {
                ID   = e.ID,
                Nome = e.Nome,
                UF   = e.UF
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar estados: {ex.Message}");
        }
    }

    public async Task<EstadoResponse> BuscarPorIdAsync(int id)
    {
        try
        {
            var estado = await _estadoRepository.GetByIdAsync(id);

            if (estado == null)
                throw new NotFoundException("Estado não encontrado.");

            return new EstadoResponse
            {
                ID   = estado.ID,
                Nome = estado.Nome,
                UF   = estado.UF
            };
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar estado: {ex.Message}");
        }
    }
}
