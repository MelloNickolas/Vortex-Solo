namespace Projeto.Application.DTOs.Vendas.Response;

public class VendaResponse
{
    public int ID { get; set; }
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public string FormaPagamento { get; set; } = string.Empty;

    public int ClienteID { get; set; }
    public string ClienteNome { get; set; } = string.Empty;

    public int UsuarioID { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;

    public List<ItemVendaResponse> Itens { get; set; } = [];
}
