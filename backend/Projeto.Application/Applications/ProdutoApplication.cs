using Projeto.Application.Common;
using Projeto.Application.DTOs.Produtos.Request;
using Projeto.Application.DTOs.Produtos.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Domain.Entities;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class ProdutoApplication : IProdutoApplication
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public ProdutoApplication(IProdutoRepository produtoRepository, ICategoriaRepository categoriaRepository)
    {
        _produtoRepository   = produtoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<PagedResponse<ProdutoResponse>> ListarPagedAsync(int page, int pageSize, string? nome, int? categoriaId)
    {


        try
        {
            // O repository retorna uma tupla (Items, Total) — não um PagedResponse

            /* Imagina que você é gerente de uma loja e pede pro seu assistente:
            "Me traz a lista de produtos da prateleira 3 e me fala quantos tem no total."
            O assistente volta com duas informações ao mesmo tempo:

            [ Lista: Arroz, Feijão, Macarrão ] [ Total: 3 ]
            
            Com tupla, o assistente já entrega as duas informações de mão em mão, sem precisar de papel:
            O assistente retorna as duas coisas de uma vez
            return (itens, total);

            Você já separa na hora que recebe
            var (itens, total) = assistente.BuscarProdutos();
            */
            var (items, total) = await _produtoRepository.GetPagedAsync(page, pageSize, nome, categoriaId);

            return new PagedResponse<ProdutoResponse>
            {
                Data     = items.Select(p => MapearResponse(p)).ToList(),
                Total    = total,
                Page     = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar produtos: {ex.Message}");
        }
    }

    public async Task<ProdutoResponse> BuscarPorIdAsync(int id)
    {
        try
        {
            var produto = await ValidarProdutoExistente(id);
            return MapearResponse(produto);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar produto: {ex.Message}");
        }
    }

    public async Task<ProdutoResponse> CriarAsync(ProdutoRequest request)
    {
        try
        {
            ValidarRequest(request);
            await ValidarCategoriaExistente(request.CategoriaID);

            var produto = new Produto
            {
                Nome          = request.Nome.Trim(),
                Descricao     = request.Descricao.Trim(),
                Preco         = request.Preco,
                EstoqueAtual  = request.EstoqueAtual,
                EstoqueMinimo = request.EstoqueMinimo,
                CategoriaID   = request.CategoriaID
            };

            await _produtoRepository.AddAsync(produto);
            return MapearResponse(produto);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao criar produto: {ex.Message}");
        }
    }

    public async Task<ProdutoResponse> AtualizarAsync(int id, ProdutoRequest request)
    {
        try
        {
            ValidarRequest(request);
            await ValidarCategoriaExistente(request.CategoriaID);

            var produto = await ValidarProdutoExistente(id);

            produto.Nome          = request.Nome.Trim();
            produto.Descricao     = request.Descricao.Trim();
            produto.Preco         = request.Preco;
            produto.EstoqueAtual  = request.EstoqueAtual;
            produto.EstoqueMinimo = request.EstoqueMinimo;
            produto.CategoriaID   = request.CategoriaID;

            await _produtoRepository.UpdateAsync(produto);
            return MapearResponse(produto);
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar produto: {ex.Message}");
        }
    }

    public async Task DeletarAsync(int id)
    {
        try
        {
            var produto = await ValidarProdutoExistente(id);
            await _produtoRepository.DeleteAsync(produto);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao deletar produto: {ex.Message}");
        }
    }

    #region Úteis

    private static void ValidarRequest(ProdutoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ValidationException("Nome do produto não pode ser vazio.");

        if (request.Preco <= 0)
            throw new ValidationException("Preço deve ser maior que zero.");

        if (request.EstoqueMinimo < 0)
            throw new ValidationException("Estoque mínimo não pode ser negativo.");

        if (request.EstoqueAtual < 0)
            throw new ValidationException("Estoque atual não pode ser negativo.");

        if (request.CategoriaID <= 0)
            throw new ValidationException("Categoria inválida.");
    }

    private async Task<Produto> ValidarProdutoExistente(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
            throw new NotFoundException("Produto não encontrado.");
        return produto;
    }

    private async Task ValidarCategoriaExistente(int categoriaId)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(categoriaId);
        if (categoria == null)
            throw new NotFoundException("Categoria informada não existe.");
    }

    // Categoria já vem carregada via Include no repository, nao precisa azer outra chamada.
    private static ProdutoResponse MapearResponse(Produto produto) => new()
    {
        ID            = produto.ID,
        Nome          = produto.Nome,
        Descricao     = produto.Descricao,
        Preco         = produto.Preco,
        EstoqueAtual  = produto.EstoqueAtual,
        EstoqueMinimo = produto.EstoqueMinimo,
        CategoriaID   = produto.CategoriaID,
        CategoriaNome = produto.Categoria?.Nome ?? string.Empty // Se o resultado for null, traz uma string vazia
    };

    #endregion
}
