namespace Projeto.Repository.FuncoesSQL;

// Modelo de retorno do Dashboard: vendas agrupadas por dia de um mês
public class VendaPorDiaResponse
{
    public int Dia { get; set; }          // dia do mês (1 a 31)
    public int Quantidade { get; set; }   // quantas vendas naquele dia
    public decimal Total { get; set; }    // valor faturado naquele dia
}
