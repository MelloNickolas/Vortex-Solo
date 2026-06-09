using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class ProdutoRepository : BaseRepository, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Produto> Items, int Total)> GetPagedAsync(
        int page, int pageSize, string? nome, int? categoriaId, bool? ativo)
    {
        try
        {
            var query = _context.Produtos
                .Include(p => p.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.Nome.ToLower().Contains(nome.ToLower()));

            if (categoriaId.HasValue)
                query = query.Where(p => p.CategoriaID == categoriaId.Value);

            if (ativo.HasValue)
                query = query.Where(p => p.Ativo == ativo.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar produtos: {ex.Message}");
        }
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.ID == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar produto {id}: {ex.Message}");
        }
    }

    // Utilizado no Dashboard para exibir alertas de estoque crítico
    public async Task<IEnumerable<Produto>> GetAbaixoDoEstoqueMinimoAsync()
    {
        try
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo && p.EstoqueAtual <= p.EstoqueMinimo)
                .OrderBy(p => p.EstoqueAtual)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar produtos com estoque crítico: {ex.Message}");
        }
    }

    public async Task<Produto> AddAsync(Produto produto)
    {
        try
        {
            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();
            return produto;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao cadastrar produto: {ex.Message}");
        }
    }

    public async Task UpdateAsync(Produto produto)
    {
        try
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar produto: {ex.Message}");
        }
    }

    // Soft delete — mantém histórico de vendas e movimentações intacto
    public async Task DeleteAsync(int id)
    {
        try
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.ID == id)
                ?? throw new KeyNotFoundException($"Produto {id} não encontrado.");

            produto.Ativo = false;
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao desativar produto: {ex.Message}");
        }
    }
}
