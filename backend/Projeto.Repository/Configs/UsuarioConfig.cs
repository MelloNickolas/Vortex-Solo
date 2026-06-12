using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Projeto.Domain.Entities;

namespace Projeto.Repository.Configs;

public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.ID);
        builder.Property(u => u.ID).ValueGeneratedOnAdd();

        builder.Property(u => u.Nome)
            .HasMaxLength(150)
            .IsRequired();

        // E-mail único
        builder.Property(u => u.Email)
            .HasMaxLength(200)
            .IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        // Hash BCrypt
        builder.Property(u => u.SenhaHash).IsRequired();
        builder.Property(u => u.Telefone).HasMaxLength(20);
        builder.Property(u => u.Ativo).HasDefaultValue(true);
    }
}
