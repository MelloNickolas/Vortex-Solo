using Projeto.Application.DTOs.Cidades.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class CidadeApplication : ICidadeApplication
{
    private readonly ICidadeRepository _cidadeRepository;

    public CidadeApplication(ICidadeRepository cidadeRepository)
    {
        _cidadeRepository = cidadeRepository;
    }

    public async Task<List<CidadeResponse>> ListarPorEstadoAsync(int estadoId)
    {
        try
        {
            var cidades = await _cidadeRepository.GetByEstadoIdAsync(estadoId);

            return cidades.Select(c => new CidadeResponse
            {
                ID       = c.ID,
                Nome     = c.Nome,
                EstadoID = c.EstadoID
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar cidades: {ex.Message}");
        }
    }

    public async Task<CidadeResponse> BuscarPorIdAsync(int id)
    {
        try
        {
            var cidade = await _cidadeRepository.GetByIdAsync(id);

            if (cidade == null)
                throw new NotFoundException("Cidade não encontrada.");

            return new CidadeResponse
            {
                ID       = cidade.ID,
                Nome     = cidade.Nome,
                EstadoID = cidade.EstadoID
            };
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar cidade: {ex.Message}");
        }
    }
}
