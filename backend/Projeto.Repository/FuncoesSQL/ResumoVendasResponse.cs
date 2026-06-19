namespace Projeto.Repository.FuncoesSQL;

// VIEW: vw_ResumoVendas
// Consolida os totais gerais de vendas do sistema.
public class ResumoVendasResponse
{
    public int TotalVendas { get; set; }
    public decimal ValorTotalGeral { get; set; }
    public decimal TicketMedio { get; set; }
    public int VendasConcluidas { get; set; }
    public int VendasCanceladas { get; set; }
}
