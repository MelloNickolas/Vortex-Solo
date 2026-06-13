using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Repository.Configs;

public class VendaConfig : IEntityTypeConfiguration<Venda>
{
    public void Configure(EntityTypeBuilder<Venda> builder)
    {
        builder.ToTable("Vendas");

        builder.HasKey(v => v.ID);
        builder.Property(v => v.ID).ValueGeneratedOnAdd();

        builder.Property(v => v.ValorTotal).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(v => v.Status).HasConversion<string>().IsRequired(true);
        builder.Property(v => v.FormaPagamento).HasConversion<string>().IsRequired(true);

        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Vendas)
            .HasForeignKey(v => v.ClienteID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Usuario)
            .WithMany(u => u.Vendas)
            .HasForeignKey(v => v.UsuarioID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
