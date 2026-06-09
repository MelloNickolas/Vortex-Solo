using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

/// <summary>
/// Apenas leitura — estados são populados diretamente no banco via SQL.
/// </summary>
public class EstadoRepository : BaseRepository, IEstadoRepository
{
    public EstadoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Estado>> GetAllAsync()
    {
        try
        {
            return await _context.Estados
                .OrderBy(e => e.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar estados: {ex.Message}");
        }
    }

    public async Task<Estado?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Estados.FirstOrDefaultAsync(e => e.ID == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar estado {id}: {ex.Message}");
        }
    }
}
