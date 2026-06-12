using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class CategoriaRepository : BaseRepository, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Categoria>> GetAllAsync()
    {
        return await _context.Categorias
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Categoria?> GetByIdAsync(int id)
    {
        return await _context.Categorias.FirstOrDefaultAsync(c => c.ID == id);
    }

    public async Task<IEnumerable<Categoria>> SearchByNameAsync(string nome)
    {
        return await _context.Categorias
            .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<Categoria> AddAsync(Categoria categoria)
    {
        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();
        return categoria;
    }

    public async Task UpdateAsync(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        await _context.SaveChangesAsync();
    }

    // Delete físico — Application busca o objeto, valida e passa aqui já resolvido
    public async Task DeleteAsync(Categoria categoria)
    {
        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();
    }
}
