namespace Projeto.Domain.Entities;

public class Categoria
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public ICollection<Produto> Produtos { get; set; } = [];
}
