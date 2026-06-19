namespace Projeto.Repository.FuncoesSQL;

// STORED PROCEDURE sp_ProdutosMaisVendidos
// Retorna os N produtos com maior quantidade vendida em vendas concluidas.
public class ProdutoMaisVendidoResponse
{
    public string Produto { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int TotalVendido { get; set; }
    public decimal TotalFaturado { get; set; }
}
