using Projeto.Domain.Enums;

namespace Projeto.Application.DTOs.Movimentacoes.Request;

public class MovimentacaoRequest
{
    public int ProdutoID { get; set; }
    public int UsuarioID { get; set; }
    public TipoMovimentacao Tipo { get; set; }

    // Apenas motivos manuais são permitidos, Venda e CancelamentoVenda são gerados automaticamente
    public MotivoMovimentacao Motivo { get; set; }

    public int Quantidade { get; set; }
}
