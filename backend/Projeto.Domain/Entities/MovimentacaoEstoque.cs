using Projeto.Domain.Enums;

namespace Projeto.Domain.Entities;

public class MovimentacaoEstoque
{
    public int ID { get; set; }
    public TipoMovimentacao Tipo { get; set; }
    public int Quantidade { get; set; }
    public MotivoMovimentacao Motivo { get; set; }
    public DateTime DataMovimento { get; set; } = DateTime.UtcNow;

    public int ProdutoID { get; set; }
    public Produto Produto { get; set; } = null!;


    public int UsuarioID { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
