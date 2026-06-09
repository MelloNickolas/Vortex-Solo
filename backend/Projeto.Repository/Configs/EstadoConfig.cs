using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Repository.Configs;

public class EstadoConfig : IEntityTypeConfiguration<Estado>
{
    public void Configure(EntityTypeBuilder<Estado> builder)
    {
        builder.ToTable("Estados");

        builder.HasKey(e => e.ID);
        builder.Property(e => e.ID).ValueGeneratedOnAdd();

        builder.Property(e => e.Nome)
            .HasMaxLength(100)
            .IsRequired();

        // Sigla de dois caracteres (ex: "SP", "RJ")
        builder.Property(e => e.UF)
            .HasMaxLength(2)
            .IsRequired();
    }
}
