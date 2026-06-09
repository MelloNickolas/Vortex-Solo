using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

/// <summary>
/// Apenas leitura — cidades são populadas diretamente no banco via SQL.
/// </summary>
public class CidadeRepository : BaseRepository, ICidadeRepository
{
    public CidadeRepository(AppDbContext context) : base(context) { }

    // Busca utilizada no formulário de cadastro de cliente: usuário seleciona o estado, então as cidades são carregadas
    public async Task<IEnumerable<Cidade>> GetByEstadoIdAsync(int estadoId)
    {
        try
        {
            return await _context.Cidades
                .Where(c => c.EstadoID == estadoId)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar cidades do estado {estadoId}: {ex.Message}");
        }
    }

    public async Task<Cidade?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Cidades.FirstOrDefaultAsync(c => c.ID == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar cidade {id}: {ex.Message}");
        }
    }
}
