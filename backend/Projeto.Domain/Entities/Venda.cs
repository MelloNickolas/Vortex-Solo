using Projeto.Domain.Enums;

namespace Projeto.Domain.Entities;

/// <summary>
/// Representa uma venda realizada no sistema.
/// Ao ser criada, os produtos do ItensVenda têm seu estoque reduzido automaticamente.
/// Ao ser cancelada (Status = Cancelada), o estoque é revertido e uma MovimentacaoEstoque
/// do tipo CancelamentoVenda é registrada para cada item.
/// </summary>
public class Venda
{
    public int ID { get; set; }

    /// <summary>Data e hora em que a venda foi registrada.</summary>
    public DateTime DataVenda { get; set; } = DateTime.UtcNow;

    /// <summary>Soma dos subtotais de todos os ItensVenda. Calculado na camada Application ao fechar a venda.</summary>
    public decimal ValorTotal { get; set; }

    /// <summary>Ciclo de vida da venda: Pendente → Concluida | Cancelada.</summary>
    public StatusVenda Status { get; set; } = StatusVenda.Pendente;

    /// <summary>Meio de pagamento utilizado pelo cliente (Dinheiro, Pix, Cartão etc.).</summary>
    public FormaPagamento FormaPagamento { get; set; }

    /// <summary>Define se o pagamento é à vista ou parcelado.</summary>
    public TipoPagamento TipoPagamento { get; set; }

    /// <summary>Número de parcelas — relevante apenas quando TipoPagamento = Parcelado.</summary>
    public int NumeroParcelas { get; set; } = 1;

    /// <summary>FK para o cliente que realizou a compra.</summary>
    public int ClienteID { get; set; }

    /// <summary>FK para o usuário que registrou a venda.</summary>
    public int UsuarioID { get; set; }

    // Propriedades de navegação
    public Cliente Cliente { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;

    // Itens que compõem esta venda (N produtos via tabela associativa)
    public ICollection<ItemVenda> Itens { get; set; } = [];
}
