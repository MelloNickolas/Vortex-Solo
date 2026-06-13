namespace Projeto.Application.DTOs.Vendas.Response;

public class ItemVendaResponse
{
    public int ID { get; set; }
    public int ProdutoID { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
