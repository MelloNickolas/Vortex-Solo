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
        try
        {
            return await _context.Categorias
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar categorias: {ex.Message}");
        }
    }

    public async Task<Categoria?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.ID == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar categoria {id}: {ex.Message}");
        }
    }

    public async Task<IEnumerable<Categoria>> SearchByNameAsync(string nome)
    {
        try
        {
            return await _context.Categorias
                .Where(c => c.Nome.ToLower().Contains(nome.ToLower()))
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar categorias por nome: {ex.Message}");
        }
    }

    public async Task<Categoria> AddAsync(Categoria categoria)
    {
        try
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar categoria: {ex.Message}");
        }
    }

    public async Task UpdateAsync(Categoria categoria)
    {
        try
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar categoria: {ex.Message}");
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.ID == id)
                ?? throw new KeyNotFoundException($"Categoria {id} não encontrada.");

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao deletar categoria: {ex.Message}");
        }
    }
}
