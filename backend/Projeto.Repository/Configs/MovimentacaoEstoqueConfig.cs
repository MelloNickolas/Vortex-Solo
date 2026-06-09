using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Repository.Configs;

public class MovimentacaoEstoqueConfig : IEntityTypeConfiguration<MovimentacaoEstoque>
{
    public void Configure(EntityTypeBuilder<MovimentacaoEstoque> builder)
    {
        builder.ToTable("MovimentacoesEstoque");

        builder.HasKey(m => m.ID);
        builder.Property(m => m.ID).ValueGeneratedOnAdd();

        // Enums armazenados como inteiro no SQLite
        builder.Property(m => m.Tipo).HasConversion<int>().IsRequired();
        builder.Property(m => m.Motivo).HasConversion<int>().IsRequired();

        // Quantidade sempre positiva — a direção é definida pelo campo Tipo
        builder.Property(m => m.Quantidade).IsRequired();

        builder.HasOne(m => m.Produto)
            .WithMany(p => p.Movimentacoes)
            .HasForeignKey(m => m.ProdutoID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Usuario)
            .WithMany(u => u.Movimentacoes)
            .HasForeignKey(m => m.UsuarioID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
