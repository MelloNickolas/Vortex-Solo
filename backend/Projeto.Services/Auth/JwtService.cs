using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Projeto.Domain.Entities;
using Projeto.Services.Interfaces;

namespace Projeto.Services.Auth;

public class JwtService : IJwtService
{ 
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GerarToken(Usuario usuario)
    {
        // Claims que serão embutidas no token — disponíveis em qualquer controller via User.Claims
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.ID.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email)
        };

        // Chave secreta lida do appsettings.json — deve ter no mínimo 32 caracteres
        var chave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Chave"]!));

        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _configuration["Jwt:Issuer"],
            audience:           _configuration["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(8), // token válido por 8 horas
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
