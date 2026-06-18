namespace Projeto.Domain.DTOs;

public class ResumoVendasResponse
{
    public int TotalVendas { get; set; }
    public decimal ValorTotalGeral { get; set; }
    public decimal TicketMedio { get; set; }
    public int VendasConcluidas { get; set; }
    public int VendasCanceladas { get; set; }
}
