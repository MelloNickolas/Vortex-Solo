namespace Projeto.Domain.Entities;

public class ItemVenda
{
    public int ID { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public int VendaID { get; set; }
    public Venda Venda { get; set; } = null!;

    public int ProdutoID { get; set; }
    public Produto Produto { get; set; } = null!;
}
