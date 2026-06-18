using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Configs;

namespace Projeto.Repository.Context;

public class AppDbContext : DbContext
{
    #region DbSets
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<Cidade> Cidades { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<ItemVenda> ItensVenda { get; set; }
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
    #endregion

    private readonly DbContextOptions? _options;

    public AppDbContext() { }

    public AppDbContext(DbContextOptions options) : base(options)
    {
        _options = options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Fallback para quando o contexto é instanciado sem injeção de dependência (ex: migrations via CLI)
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-2T4L4UE\SQLEXPRESS;Database=VortexDB;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsuarioConfig());
        modelBuilder.ApplyConfiguration(new EstadoConfig());
        modelBuilder.ApplyConfiguration(new CidadeConfig());
        modelBuilder.ApplyConfiguration(new CategoriaConfig());
        modelBuilder.ApplyConfiguration(new ProdutoConfig());
        modelBuilder.ApplyConfiguration(new ClienteConfig());
        modelBuilder.ApplyConfiguration(new VendaConfig());
        modelBuilder.ApplyConfiguration(new ItemVendaConfig());
        modelBuilder.ApplyConfiguration(new MovimentacaoEstoqueConfig());
    }
}
