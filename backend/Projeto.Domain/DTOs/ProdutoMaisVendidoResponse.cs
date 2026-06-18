namespace Projeto.Domain.DTOs;

public class ProdutoMaisVendidoResponse
{
    public string Produto { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int TotalVendido { get; set; }
    public decimal TotalFaturado { get; set; }
}
