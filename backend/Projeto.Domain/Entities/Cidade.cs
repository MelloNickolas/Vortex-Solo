namespace Projeto.Domain.Entities;

public class Cidade
{
    public int ID { get; set; }
    public string Nome { get; set; } = string.Empty;

    public int EstadoID { get; set; }
    public Estado Estado { get; set; } = null!;
    
    public ICollection<Cliente> Clientes { get; set; } = [];
}
