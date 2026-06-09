using Projeto.Repository.Context;

namespace Projeto.Repository.Repositories;

/// <summary>
/// Classe base para todos os repositórios.
/// Injeta o AppDbContext e o disponibiliza como campo protegido para as subclasses.
/// </summary>
public abstract class BaseRepository
{
    protected readonly AppDbContext _context;

    protected BaseRepository(AppDbContext context)
    {
        _context = context;
    }
}
