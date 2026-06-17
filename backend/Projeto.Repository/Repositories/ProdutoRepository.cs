using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;

namespace Projeto.Repository.Repositories;

public class ProdutoRepository : BaseRepository, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context) { }

    public async Task<(IEnumerable<Produto> Items, int Total)> GetPagedAsync(
        int page, int pageSize, string? nome, int? categoriaId)
    {
        // imagina que voce abriu um armário, mas nao pegou nada ainda, só abriu a porta.
        var query = _context.Produtos
            .Include(p => p.Categoria)
            .AsQueryable(); // aqui ele siginifica que nada foi executado no banco, é só uma receita

        // aqui passamos os filtros opcionais....
        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(p => p.Nome.ToLower().Contains(nome.ToLower()));
        if (categoriaId.HasValue)
            query = query.Where(p => p.CategoriaID == categoriaId.Value);

        // sele vai contar depois de tudo quantos produtos tem ao todo, ai o nosso front-end só usa esse numero .
        var total = await query.CountAsync();

        
        var items = await query
            // primeiro ordenamos cada produto por ordem alfabetica.
            .OrderBy(p => p.Nome) 
            /* A fórmula (page - 1) * pageSize calcula exatamente quantos pular para chegar na página certa. 
            vamos supor que a pageSize é 10, e o front quer a page 2, o cálculo vai ficar assim:
            (2 - 1) * 10 = 10 ----- ou seja, ele vai pular os 10 primeiros registros. */
            .Skip((page - 1) * pageSize)
            // vai pegar os 10 itens depois de pular.
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.ID == id);
    }

    // Utilizado no Dashboard para exibir alertas de estoque crítico
    public async Task<IEnumerable<Produto>> GetAbaixoDoEstoqueMinimoAsync()
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => p.EstoqueAtual <= p.EstoqueMinimo)
            .OrderBy(p => p.EstoqueAtual)
            .ToListAsync();
    }

    public async Task<Produto> AddAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    public async Task UpdateAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Produto produto)
    {
        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
    }
}
