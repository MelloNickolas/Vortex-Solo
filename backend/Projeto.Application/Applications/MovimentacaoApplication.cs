using Projeto.Application.Common;
using Projeto.Application.DTOs.Movimentacoes.Request;
using Projeto.Application.DTOs.Movimentacoes.Response;
using Projeto.Application.Exceptions;
using Projeto.Application.Interfaces;
using Projeto.Domain.Entities;
using Projeto.Domain.Enums;
using Projeto.Repository.Interfaces;

namespace Projeto.Application.Applications;

public class MovimentacaoApplication : IMovimentacaoApplication
{
    private readonly IMovimentacaoEstoqueRepository _movimentacaoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public MovimentacaoApplication(
        IMovimentacaoEstoqueRepository movimentacaoRepository,
        IProdutoRepository produtoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _movimentacaoRepository = movimentacaoRepository;
        _produtoRepository      = produtoRepository;
        _usuarioRepository      = usuarioRepository;
    }

    public async Task<PagedResponse<MovimentacaoResponse>> ListarPagedAsync(
        int page, int pageSize, int? produtoId, string? tipo, DateTime? de, DateTime? ate)
    {
        try
        {
            TipoMovimentacao? tipoEnum = null;
            if (!string.IsNullOrWhiteSpace(tipo) && Enum.TryParse<TipoMovimentacao>(tipo, out var parsed))
                tipoEnum = parsed;

            var (items, total) = await _movimentacaoRepository.GetPagedAsync(page, pageSize, produtoId, tipoEnum, de, ate);

            return new PagedResponse<MovimentacaoResponse>
            {
                Data     = items.Select(m => MapearResponse(m)).ToList(),
                Total    = total,
                Page     = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao listar movimentações: {ex.Message}");
        }
    }

    public async Task<MovimentacaoResponse> CriarAsync(MovimentacaoRequest request)
    {
        try
        {
            ValidarRequest(request);

            var produto = await _produtoRepository.GetByIdAsync(request.ProdutoID);
            if (produto == null)
                throw new NotFoundException("Produto não encontrado.");

            var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioID);
            if (usuario == null)
                throw new NotFoundException("Usuário não encontrado.");

            // Saída não pode deixar o estoque negativo
            if (request.Tipo == TipoMovimentacao.Saida && produto.EstoqueAtual < request.Quantidade)
                throw new ValidationException($"Estoque insuficiente. Disponível: {produto.EstoqueAtual}.");

            // Atualiza o estoque conforme o tipo da movimentação
            if (request.Tipo == TipoMovimentacao.Entrada)
                produto.EstoqueAtual += request.Quantidade;
            else
                produto.EstoqueAtual -= request.Quantidade;

            await _produtoRepository.UpdateAsync(produto);

            var movimentacao = await _movimentacaoRepository.AddAsync(new MovimentacaoEstoque
            {
                Tipo          = request.Tipo,
                Motivo        = request.Motivo,
                Quantidade    = request.Quantidade,
                DataMovimento = DateTime.UtcNow,
                ProdutoID     = request.ProdutoID,
                UsuarioID     = request.UsuarioID
            });

            return MapearResponse(movimentacao);
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
            throw new Exception($"Erro ao registrar movimentação: {ex.Message}");
        }
    }

    #region Úteis

    private static void ValidarRequest(MovimentacaoRequest request)
    {
        if (request.ProdutoID <= 0)
            throw new ValidationException("Produto inválido.");

        if (request.UsuarioID <= 0)
            throw new ValidationException("Usuário inválido.");

        if (request.Quantidade <= 0)
            throw new ValidationException("Quantidade deve ser maior que zero.");

        // Impede usar motivos que são exclusivos do sistema automático
        var motivosProibidos = new[] { MotivoMovimentacao.Venda, MotivoMovimentacao.CancelamentoVenda };
        if (motivosProibidos.Contains(request.Motivo))
            throw new ValidationException("Motivos 'Venda' e 'CancelamentoVenda' são gerados automaticamente pelo sistema.");
    }

    private static MovimentacaoResponse MapearResponse(MovimentacaoEstoque m) => new()
    {
        ID            = m.ID,
        Tipo          = m.Tipo.ToString(),
        Motivo        = m.Motivo.ToString(),
        Quantidade    = m.Quantidade,
        DataMovimento = m.DataMovimento,
        ProdutoID     = m.ProdutoID,
        ProdutoNome   = m.Produto?.Nome ?? string.Empty,
        UsuarioID     = m.UsuarioID,
        UsuarioNome   = m.Usuario?.Nome ?? string.Empty
    };

    #endregion
}
