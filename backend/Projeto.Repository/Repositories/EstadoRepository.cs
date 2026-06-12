using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

// Apenas leitura — estados são populados diretamente no banco via SQL
public class EstadoRepository : BaseRepository, IEstadoRepository
{
    public EstadoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Estado>> GetAllAsync()
    {
        return await _context.Estados
            .OrderBy(e => e.Nome)
            .ToListAsync();
    }

    public async Task<Estado?> GetByIdAsync(int id)
    {
        return await _context.Estados.FirstOrDefaultAsync(e => e.ID == id);
    }
}
