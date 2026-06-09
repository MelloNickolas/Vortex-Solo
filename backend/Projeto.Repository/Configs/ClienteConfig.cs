using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Repository.Configs;

public class ClienteConfig : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.ID);
        builder.Property(c => c.ID).ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .HasMaxLength(150)
            .IsRequired();

        // CPF armazenado sem formatação, apenas dígitos (11 caracteres)
        builder.Property(c => c.CPF).HasMaxLength(11);

        builder.Property(c => c.Telefone).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Rua).HasMaxLength(200);

        // Número como string para suportar valores como "S/N" ou "123A"
        builder.Property(c => c.Numero).HasMaxLength(20);

        builder.Property(c => c.Ativo).HasDefaultValue(true);

        builder.HasOne(c => c.Cidade)
            .WithMany(ci => ci.Clientes)
            .HasForeignKey(c => c.CidadeID)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
