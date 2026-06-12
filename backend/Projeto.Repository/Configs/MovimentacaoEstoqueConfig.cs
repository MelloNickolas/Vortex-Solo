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

        builder.Property(m => m.Tipo).HasConversion<string>().IsRequired(true);
        builder.Property(m => m.Motivo).HasConversion<string>().IsRequired(true);

        // quanto desse item foi movido?
        builder.Property(m => m.Quantidade).IsRequired(true);

        // Como vai ter movimentações registradas com produtos que não existem?
        builder.HasOne(m => m.Produto)
            .WithMany(p => p.Movimentacoes)
            .HasForeignKey(m => m.ProdutoID)
            .OnDelete(DeleteBehavior.Restrict);

        // o mesmo vale para os ususários né.
        builder.HasOne(m => m.Usuario)
            .WithMany(u => u.Movimentacoes)
            .HasForeignKey(m => m.UsuarioID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
