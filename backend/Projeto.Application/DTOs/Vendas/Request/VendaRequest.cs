using Projeto.Domain.Enums;

namespace Projeto.Application.DTOs.Vendas.Request;

public class VendaRequest
{
    public int ClienteID { get; set; }
    public int UsuarioID { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public List<ItemVendaRequest> Itens { get; set; } = [];
}
