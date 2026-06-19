namespace Projeto.Repository.FuncoesSQL;

// VIEW vw_VendasPorFormaPagamento
// Agrupa vendas concluidas por forma de pagamento com quantidade e total.
public class VendaPorFormaPagamentoResponse
{
    public string FormaPagamentoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Total { get; set; }
}
