namespace Projeto.Domain.Entities;

public class Cliente
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rua { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true; // para SOFT DELETE

    public int CidadeID { get; set; }
    public Cidade Cidade { get; set; } = null!;

    public ICollection<Venda> Vendas { get; set; } = [];
}
