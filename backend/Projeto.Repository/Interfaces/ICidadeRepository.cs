using Projeto.Domain.Entities;

namespace Projeto.Repository.Interfaces;

/// Apenas leitura, as cidades são inseridas diretamente no banco, sem CRUD pela API.
public interface ICidadeRepository
{
    Task<IEnumerable<Cidade>> GetByEstadoIdAsync(int estadoId);
    Task<Cidade?> GetByIdAsync(int id);
}
