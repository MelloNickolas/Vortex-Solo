namespace Projeto.Domain.DTOs;

public class VendaPorFormaPagamentoResponse
{
    public string FormaPagamentoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal Total { get; set; }
}
