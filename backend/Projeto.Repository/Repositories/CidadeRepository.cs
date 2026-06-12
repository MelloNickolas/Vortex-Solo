using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class CidadeRepository : BaseRepository, ICidadeRepository
{
    public CidadeRepository(AppDbContext context) : base(context) { }

    // usuário seleciona o estado, as cidades daquele estado aparecem
    // vamos usar para filtrar
    public async Task<IEnumerable<Cidade>> GetByEstadoIdAsync(int estadoId)
    {
        return await _context.Cidades
            .Where(c => c.EstadoID == estadoId)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Cidade?> GetByIdAsync(int id)
    {
        return await _context.Cidades.FirstOrDefaultAsync(c => c.ID == id);
    }
}
