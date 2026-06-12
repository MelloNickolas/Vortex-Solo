using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Repository.Configs;

public class CidadeConfig : IEntityTypeConfiguration<Cidade>
{
    public void Configure(EntityTypeBuilder<Cidade> builder)
    {
        builder.ToTable("Cidades");

        builder.HasKey(c => c.ID);
        builder.Property(c => c.ID).ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .HasMaxLength(150)
            .IsRequired();

        builder.HasOne(c => c.Estado)
            .WithMany(e => e.Cidades)
            .HasForeignKey(c => c.EstadoID);
    }
}
