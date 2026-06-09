using Projeto.Domain.Entities;

namespace Projeto.Repository.Interfaces;

/// Apenas leitura — estados são inseridos diretamente no banco, sem CRUD pela API.
public interface IEstadoRepository
{
    Task<IEnumerable<Estado>> GetAllAsync();
    Task<Estado?> GetByIdAsync(int id);
}
