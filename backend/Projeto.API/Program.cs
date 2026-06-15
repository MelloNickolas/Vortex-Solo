using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Projeto.API.Middlewares;
using Projeto.Application.Applications;
using Projeto.Application.Interfaces;
using Projeto.Domain.Entities;
using Projeto.Repository.Context;
using Projeto.Repository.Interfaces;
using Projeto.Repository.Repositories;
using Projeto.Services.Auth;
using Projeto.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── Applications
builder.Services.AddScoped<IEstadoApplication, EstadoApplication>();
builder.Services.AddScoped<ICidadeApplication, CidadeApplication>();
builder.Services.AddScoped<ICategoriaApplication, CategoriaApplication>();
builder.Services.AddScoped<IUsuarioApplication, UsuarioApplication>();
builder.Services.AddScoped<IProdutoApplication, ProdutoApplication>();
builder.Services.AddScoped<IClienteApplication, ClienteApplication>();
builder.Services.AddScoped<IAuthApplication, AuthApplication>();
builder.Services.AddScoped<IVendaApplication, VendaApplication>();
builder.Services.AddScoped<IMovimentacaoApplication, MovimentacaoApplication>();

// ── Repositories 
builder.Services.AddScoped<IEstadoRepository, EstadoRepository>();
builder.Services.AddScoped<ICidadeRepository, CidadeRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IMovimentacaoEstoqueRepository, MovimentacaoEstoqueRepository>();

// ── Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IBCryptService, BCryptService>();

// ── Banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── CORS 
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Controllers e o Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Informe o token JWT. Exemplo: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ── JWT
var jwtChave = builder.Configuration["Jwt:Chave"]!;
var chave = Encoding.UTF8.GetBytes(jwtChave);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken            = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey         = new SymmetricSecurityKey(chave),
        ValidateIssuer           = true,
        ValidIssuer              = builder.Configuration["Jwt:Issuer"],
        ValidateAudience         = true,
        ValidAudience            = builder.Configuration["Jwt:Audience"]
    };
});

var app = builder.Build();

// ── Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Usuarios.Any(u => u.Email == "admin@vortex.com"))
    {
        db.Usuarios.Add(new Usuario
        {
            Nome      = "Administrador",
            Email     = "admin@vortex.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Telefone  = "",
            CriadoEm = DateTime.UtcNow,
            Ativo     = true
        });

        db.SaveChanges();
    }
}

// ── Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
