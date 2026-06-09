namespace Projeto.Domain.Entities;

public class Estado
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;

    public ICollection<Cidade> Cidades { get; set; } = [];
}
