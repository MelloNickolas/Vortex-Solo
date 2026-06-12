using Projeto.Application.DTOs.Categorias.Request;
using Projeto.Application.DTOs.Categorias.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Domain.Entities;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class CategoriaApplication : ICategoriaApplication
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaApplication(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<List<CategoriaResponse>> ListarAsync()
    {
        try
        {
            var categorias = await _categoriaRepository.GetAllAsync();

            return categorias.Select(c => new CategoriaResponse
            {
                ID   = c.ID,
                Nome = c.Nome
            }).ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar categorias: {ex.Message}");
        }
    }

    public async Task<CategoriaResponse> BuscarPorIdAsync(int id)
    {
        try
        {
            var categoria = await ValidarCategoriaExistente(id);

            return new CategoriaResponse
            {
                ID   = categoria.ID,
                Nome = categoria.Nome
            };
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar categoria: {ex.Message}");
        }
    }

    public async Task<CategoriaResponse> CriarAsync(CategoriaRequest request)
    {
        try
        {
            ValidarRequest(request);
            var categoria = new Categoria
            {
                //Sem o Trim(), salvaria com os espaços no banco — aí você teria categorias como "Eletrônicos" e " Eletrônicos "
                Nome = request.Nome.Trim()
            };

            await _categoriaRepository.AddAsync(categoria);

            return new CategoriaResponse
            {
                ID   = categoria.ID,
                Nome = categoria.Nome
            };
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao criar categoria: {ex.Message}");
        }
    }

    public async Task<CategoriaResponse> AtualizarAsync(int id, CategoriaRequest request)
    {
        try
        {
            ValidarRequest(request);

            var categoria = await ValidarCategoriaExistente(id);
            categoria.Nome = request.Nome.Trim();

            await _categoriaRepository.UpdateAsync(categoria);

            return new CategoriaResponse
            {
                ID   = categoria.ID,
                Nome = categoria.Nome
            };
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (ValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao atualizar categoria: {ex.Message}");
        }
    }

    public async Task DeletarAsync(int id)
    {
        try
        {
            var categoria = await ValidarCategoriaExistente(id);
            await _categoriaRepository.DeleteAsync(categoria);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao deletar categoria: {ex.Message}");
        }
    }

    #region Úteis
    private static void ValidarRequest(CategoriaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ValidationException("Nome da categoria não pode ser vazio.");

        if (request.Nome.Length > 100)
            throw new ValidationException("Nome da categoria não pode ter mais de 100 caracteres.");
    }

    private async Task<Categoria> ValidarCategoriaExistente(int id)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id);
        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada.");
            
        return categoria;
    }

    #endregion
}
