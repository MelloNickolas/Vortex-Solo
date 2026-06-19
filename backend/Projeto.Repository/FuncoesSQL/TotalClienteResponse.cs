namespace Projeto.Repository.FuncoesSQL;

// Modelo de retorno da FUNCTION: fn_TotalVendasCliente(@ClienteID)
// Retorna o valor total faturado em vendas concluidas de um cliente especifico.
public class TotalClienteResponse
{
    public decimal Total { get; set; }
}
