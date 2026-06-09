using Microsoft.EntityFrameworkCore;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;

var builder = WebApplication.CreateBuilder(args);

// ── Banco de dados ─────────────────────────────────────────────────────────────
// SQLite — o arquivo é criado na raiz do projeto da API durante a execução
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Controllers + Swagger ──────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── Migrations + Seed do usuário padrão ───────────────────────────────────────
// Executa automaticamente ao subir a aplicação:
//   1. Aplica migrations pendentes (cria o banco se não existir)
//   2. Insere o usuário admin padrão caso ainda não exista
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Aplica migrations pendentes automaticamente
    db.Database.Migrate();

    // Seed: garante que exista pelo menos um usuário admin ativo
    if (!db.Usuarios.Any(u => u.Email == "admin@vortex.com"))
    {
        db.Usuarios.Add(new Usuario
        {
            Nome      = "Administrador",
            Email     = "admin@vortex.com",
            // Senha padrão: admin123 — deve ser trocada no primeiro acesso
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Telefone  = "",
            CriadoEm = DateTime.UtcNow,
            Ativo     = true
        });

        db.SaveChanges();
    }
}

// ── Pipeline HTTP ──────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
