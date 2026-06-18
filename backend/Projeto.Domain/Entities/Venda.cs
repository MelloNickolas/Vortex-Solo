using Projeto.Domain.Enums;

namespace Projeto.Domain.Entities;

public class Venda
{
    public int ID { get; set; }
    public DateTime DataVenda { get; set; } = DateTime.UtcNow;
    public decimal ValorTotal { get; set; }
    public StatusVenda Status { get; set; } = StatusVenda.Concluida;
    public FormaPagamento FormaPagamento { get; set; }

    public int ClienteID { get; set; }
    public int UsuarioID { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public ICollection<ItemVenda> Itens { get; set; } = [];
}
