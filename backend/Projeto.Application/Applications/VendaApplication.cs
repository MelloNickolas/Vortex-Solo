using Projeto.Application.Common;
using Projeto.Application.DTOs.Vendas.Request;
using Projeto.Application.DTOs.Vendas.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Domain.Entities;
using Projeto.Domain.Enums;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class VendaApplication : IVendaApplication
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;

    public VendaApplication(
        IVendaRepository vendaRepository,
        IProdutoRepository produtoRepository,
        IClienteRepository clienteRepository,
        IUsuarioRepository usuarioRepository,
        IMovimentacaoEstoqueRepository movimentacaoRepository)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
        _clienteRepository = clienteRepository;
        _usuarioRepository = usuarioRepository;
        _movimentacaoRepository = movimentacaoRepository;
    }

    public async Task<PagedResponse<VendaResponse>> ListarPagedAsync(int page, int pageSize, string? status, DateTime? de, DateTime? ate)
    {
        try
        {
            // Converte a string do filtro para o enum
            /*
            Tenta converter uma string para um enum. Retorna true se conseguiu, false se não conseguiu
            
            É como ligar para o banco perguntando se um cheque é válido. O atendente te responde duas coisas ao mesmo tempo:

            Retorno: "Sim, é válido" (true) ou "Não, é inválido" (false)
            out: "O valor do cheque é R$ 500" (parsed = StatusVenda.Concluida)
            Se o cheque for inválido, ele responde false e você ignora o valor — o out existiu mas você não usa.
            
            
            
            StatusVenda? statusEnum = null; // começa sem filtro

            if (!string.IsNullOrWhiteSpace(status)              // 1. veio alguma string?
                && Enum.TryParse<StatusVenda>(status, out var parsed)) // 2. é um status válido?
                statusEnum = parsed;                            // 3. só aí aplica o filtro

            Exemplos:
            status = null       → statusEnum = null     → lista todas as vendas
            status = "Concluida"→ statusEnum = Concluida → filtra só concluídas
            status = "Qualquer" → statusEnum = null     → string inválida, ignora    
            
            */
            
            StatusVenda? statusEnum = null;
            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<StatusVenda>(status, out var parsed))
                statusEnum = parsed;

            var (items, total) = await _vendaRepository.GetPagedAsync(page, pageSize, statusEnum, de, ate);

            return new PagedResponse<VendaResponse>
            {
                Data = items.Select(v => MapearResponse(v)).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar vendas: {ex.Message}");
        }
    }

    public async Task<VendaResponse> BuscarPorIdAsync(int id)
    {
        try
        {
            var venda = await ValidarVendaExistente(id);
            return MapearResponse(venda);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao buscar venda: {ex.Message}");
        }
    }

    public async Task<VendaResponse> CriarAsync(VendaRequest request)
    {
        try
        {
            ValidarRequest(request);

            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteID);
            if (cliente == null)
                throw new NotFoundException("Cliente não encontrado.");
            if (!cliente.Ativo)
                throw new ValidationException("Cliente inativo.");


            var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioID);
            if (usuario == null)
                throw new NotFoundException("Usuário não encontrado.");


            // Valida estoque e monta os itens — dois passos em um só loop
            var itens = new List<ItemVenda>();
            decimal valorTotal = 0;

            foreach (var itemRequest in request.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(itemRequest.ProdutoID);
                if (produto == null)
                    throw new NotFoundException($"Produto ID {itemRequest.ProdutoID} não encontrado.");

                if (produto.EstoqueAtual < itemRequest.Quantidade)
                    throw new ValidationException($"Estoque insuficiente para o produto '{produto.Nome}'. Disponível: {produto.EstoqueAtual}.");

                var subtotal = produto.Preco * itemRequest.Quantidade;
                valorTotal += subtotal;

                itens.Add(new ItemVenda
                {
                    ProdutoID = produto.ID,
                    Quantidade = itemRequest.Quantidade,
                    PrecoUnitario = produto.Preco,
                    Subtotal = subtotal
                });
            }

            // Monta e salva a venda com os itens
            var venda = new Venda
            {
                DataVenda = DateTime.UtcNow,
                ValorTotal = valorTotal,
                Status = StatusVenda.Concluida,
                FormaPagamento = request.FormaPagamento,
                ClienteID = request.ClienteID,
                UsuarioID = request.UsuarioID,
                Itens = itens
            };

            await _vendaRepository.AddAsync(venda);

            // Desconta o estoque e registra movimentação para cada produto vendido
            foreach (var item in itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoID);
                produto!.EstoqueAtual -= item.Quantidade; // ! = garanto que não null, pode acessar
                await _produtoRepository.UpdateAsync(produto);

                await _movimentacaoRepository.AddAsync(new MovimentacaoEstoque
                {
                    Tipo = TipoMovimentacao.Saida,
                    Quantidade = item.Quantidade,
                    Motivo = MotivoMovimentacao.Venda,
                    DataMovimento = DateTime.UtcNow,
                    ProdutoID = item.ProdutoID,
                    UsuarioID = request.UsuarioID
                });
            }

            return MapearResponse(venda);
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
            throw new Exception($"Erro ao criar venda: {ex.Message}");
        }
    }

    public async Task<VendaResponse> CancelarAsync(int id, int usuarioId)
    {
        try
        {
            var venda = await ValidarVendaExistente(id);

            if (venda.Status == StatusVenda.Cancelada)
                throw new ValidationException("Venda já está cancelada.");

            venda.Status = StatusVenda.Cancelada;
            await _vendaRepository.UpdateAsync(venda);

            // Devolve o estoque de cada produto e registra a movimentação
            foreach (var item in venda.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(item.ProdutoID);
                produto!.EstoqueAtual += item.Quantidade; // produto! = garanto que não é null pode acessa
                await _produtoRepository.UpdateAsync(produto);

                await _movimentacaoRepository.AddAsync(new MovimentacaoEstoque
                {
                    Tipo = TipoMovimentacao.Entrada,
                    Quantidade = item.Quantidade,
                    Motivo = MotivoMovimentacao.CancelamentoVenda,
                    DataMovimento = DateTime.UtcNow,
                    ProdutoID = item.ProdutoID,
                    UsuarioID = usuarioId
                });
            }

            return MapearResponse(venda);
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
            throw new Exception($"Erro ao cancelar venda: {ex.Message}");
        }
    }

    #region Úteis

    private static void ValidarRequest(VendaRequest request)
    {
        if (request.ClienteID <= 0)
            throw new ValidationException("Cliente inválido.");

        if (request.UsuarioID <= 0)
            throw new ValidationException("Usuário inválido.");

        if (request.Itens == null || request.Itens.Count == 0)
            throw new ValidationException("A venda deve ter pelo menos um item.");

        foreach (var item in request.Itens)
        {
            if (item.Quantidade <= 0)
                throw new ValidationException("Quantidade dos itens deve ser maior que zero.");
        }
    }

    private async Task<Venda> ValidarVendaExistente(int id)
    {
        var venda = await _vendaRepository.GetByIdAsync(id);
        if (venda == null)
            throw new NotFoundException("Venda não encontrada.");
        return venda;
    }

    private static VendaResponse MapearResponse(Venda venda) => new()
    {
        ID = venda.ID,
        DataVenda = venda.DataVenda,
        ValorTotal = venda.ValorTotal,
        Status = venda.Status.ToString(),
        FormaPagamento = venda.FormaPagamento.ToString(),
        ClienteID = venda.ClienteID,
        ClienteNome = venda.Cliente?.Nome ?? string.Empty,
        UsuarioID = venda.UsuarioID,
        UsuarioNome = venda.Usuario?.Nome ?? string.Empty,
        Itens = venda.Itens?.Select(i => new ItemVendaResponse // se itens for nulo nem tenta dar o select
        {
            ID = i.ID,
            ProdutoID = i.ProdutoID,
            ProdutoNome = i.Produto?.Nome ?? string.Empty,
            Quantidade = i.Quantidade,
            PrecoUnitario = i.PrecoUnitario,
            Subtotal = i.Subtotal
        }).ToList() ?? [] // se vier null retorna uma lista vazia
    };

    #endregion
}
