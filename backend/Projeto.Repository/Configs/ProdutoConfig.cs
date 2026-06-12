using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Repository.Configs;

public class ProdutoConfig : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.ID);
        builder.Property(p => p.ID).ValueGeneratedOnAdd();

        builder.Property(p => p.Nome)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Descricao).HasMaxLength(1000);

        // Precisão decimal para evitar erros de arredondamento em valores monetários
        builder.Property(p => p.Preco)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // vai deletar a categoria e deixar os produtos ja cadastrado com a relação?
        // vao ter produtos sem categoria entao...
        builder.HasOne(p => p.Categoria)
            .WithMany(c => c.Produtos)
            .HasForeignKey(p => p.CategoriaID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
