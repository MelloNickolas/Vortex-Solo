using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Repository.Configs;

public class ItemVendaConfig : IEntityTypeConfiguration<ItemVenda>
{
    public void Configure(EntityTypeBuilder<ItemVenda> builder)
    {
        builder.ToTable("ItensVenda");

        builder.HasKey(i => i.ID);
        builder.Property(i => i.ID).ValueGeneratedOnAdd();

        builder.Property(i => i.PrecoUnitario)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(i => i.Subtotal)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        // Ao deletar a venda, os itens são removidos em cascata
        // pra entender melhor, nao tem como ter itens venda se aquela venda nem existe sacou?
        builder.HasOne(i => i.Venda)
            .WithMany(v => v.Itens)
            .HasForeignKey(i => i.VendaID)
            .OnDelete(DeleteBehavior.Cascade);

        // Produto não pode ser deletado enquanto houver itens de venda referenciado
        // Imagina que voce vendeu um Notebook, se voce deletar esse produto, essa tabela vai referenciar qual item?
        // nenhum né?
        builder.HasOne(i => i.Produto)
            .WithMany(p => p.ItensVenda)
            .HasForeignKey(i => i.ProdutoID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
