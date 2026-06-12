using Projeto.Repository.Context;

namespace Projeto.Repository.Repositories;

public abstract class BaseRepository
{
    protected readonly AppDbContext _context;

    protected BaseRepository(AppDbContext context)
    {
        _context = context;
    }
}
