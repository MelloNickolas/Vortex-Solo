using Projeto.Application.Common;
using Projeto.Application.DTOs.Clientes.Request;
using Projeto.Application.DTOs.Clientes.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Domain.Entities;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class ClienteApplication : IClienteApplication
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ICidadeRepository _cidadeRepository;

    public ClienteApplication(IClienteRepository clienteRepository, ICidadeRepository cidadeRepository)
    {
        _clienteRepository = clienteRepository;
        _cidadeRepository  = cidadeRepository;
    }

    public async Task<PagedResponse<ClienteResponse>> ListarPagedAsync(int page, int pageSize, string? busca, bool? ativo)
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
            var (items, total) = await _clienteRepository.GetPagedAsync(page, pageSize, busca, ativo);

            return new PagedResponse<ClienteResponse>
            {
                Data     = items.Select(c => MapearResponse(c)).ToList(),
                Total    = total,
                Page     = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar clientes: {ex.Message}");
        }
    }

    public async Task<ClienteResponse> BuscarPorIdAsync(int id)
    {
        try
        {
            var cliente = await ValidarClienteExistente(id);
            return MapearResponse(cliente);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar cliente: {ex.Message}");
        }
    }

    public async Task<ClienteResponse> CriarAsync(ClienteRequest request)
    {
        try
        {
            ValidarRequest(request);
            await ValidarCpfDisponivel(request.CPF);
            await ValidarCidadeExistente(request.CidadeID);

            var cliente = new Cliente
            {
                Nome     = request.Nome.Trim(),
                CPF      = request.CPF.Trim(),
                Telefone = request.Telefone.Trim(),
                Email    = request.Email.Trim().ToLower(),
                Rua      = request.Rua.Trim(),
                Numero   = request.Numero.Trim(),
                CidadeID = request.CidadeID,
                Ativo    = true
            };

            await _clienteRepository.AddAsync(cliente);
            return MapearResponse(cliente);
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
            throw new Exception($"Erro ao criar cliente: {ex.Message}");
        }
    }

    public async Task<ClienteResponse> AtualizarAsync(int id, ClienteRequest request)
    {
        try
        {
            ValidarRequest(request);
            await ValidarCidadeExistente(request.CidadeID);

            var cliente = await ValidarClienteExistente(id);

            // Se o CPF mudou, valida se o novo CPF já está em uso
            if (!cliente.CPF.Equals(request.CPF.Trim()))
                await ValidarCpfDisponivel(request.CPF);

            cliente.Nome     = request.Nome.Trim();
            cliente.CPF      = request.CPF.Trim();
            cliente.Telefone = request.Telefone.Trim();
            cliente.Email    = request.Email.Trim().ToLower();
            cliente.Rua      = request.Rua.Trim();
            cliente.Numero   = request.Numero.Trim();
            cliente.CidadeID = request.CidadeID;

            await _clienteRepository.UpdateAsync(cliente);
            return MapearResponse(cliente);
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
            throw new Exception($"Erro ao atualizar cliente: {ex.Message}");
        }
    }

    public async Task DeletarAsync(int id)
    {
        try
        {
            var cliente = await ValidarClienteExistente(id);
            // Soft delete — marca como inativo, não remove do banco
            // Histórico de vendas do cliente é preservado
            await _clienteRepository.DeleteAsync(cliente);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao deletar cliente: {ex.Message}");
        }
    }

    #region Úteis

    private static void ValidarRequest(ClienteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            throw new ValidationException("Nome do cliente não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(request.CPF))
            throw new ValidationException("CPF não pode ser vazio.");

        if (request.CPF.Trim().Length != 11)
            throw new ValidationException("CPF deve ter 11 dígitos.");

        if (string.IsNullOrWhiteSpace(request.Telefone))
            throw new ValidationException("Telefone não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(request.Rua))
            throw new ValidationException("Rua não pode ser vazia.");

        if (string.IsNullOrWhiteSpace(request.Numero))
            throw new ValidationException("Número não pode ser vazio.");

        if (request.CidadeID <= 0)
            throw new ValidationException("Cidade inválida.");
    }

    private async Task ValidarCpfDisponivel(string cpf)
    {
        var existe = await _clienteRepository.GetByCpfAsync(cpf.Trim());
        if (existe != null)
            throw new ValidationException("Já existe um cliente com esse CPF.");
    }

    private async Task ValidarCidadeExistente(int cidadeId)
    {
        var cidade = await _cidadeRepository.GetByIdAsync(cidadeId);
        if (cidade == null)
            throw new NotFoundException("Cidade informada não existe.");
    }

    private async Task<Cliente> ValidarClienteExistente(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente == null)
            throw new NotFoundException("Cliente não encontrado.");
        return cliente;
    }

    // Cidade já vem carregada via Include no repository — sem chamada extra ao banco
    private static ClienteResponse MapearResponse(Cliente cliente) => new()
    {
        ID         = cliente.ID,
        Nome       = cliente.Nome,
        CPF        = cliente.CPF,
        Telefone   = cliente.Telefone,
        Email      = cliente.Email,
        Rua        = cliente.Rua,
        Numero     = cliente.Numero,
        Ativo      = cliente.Ativo,
        CidadeID   = cliente.CidadeID,
        CidadeNome = cliente.Cidade?.Nome ?? string.Empty
    };

    #endregion
}
